﻿// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// Microsoft Public License (MS-PL)
// 
// This license governs use of the accompanying software. If you use the
// software, you accept this license. If you do not accept the license, do not
// use the software.
// 
// 1. Definitions
// 
//   The terms "reproduce," "reproduction," "derivative works," and
//   "distribution" have the same meaning here as under U.S. copyright law. A
//   "contribution" is the original software, or any additions or changes to
//   the software. A "contributor" is any person that distributes its
//   contribution under this license. "Licensed patents" are a contributor's
//   patent claims that read directly on its contribution.
// 
// 2. Grant of Rights
// 
//   (A) Copyright Grant- Subject to the terms of this license, including the
//       license conditions and limitations in section 3, each contributor
//       grants you a non-exclusive, worldwide, royalty-free copyright license
//       to reproduce its contribution, prepare derivative works of its
//       contribution, and distribute its contribution or any derivative works
//       that you create.
// 
//   (B) Patent Grant- Subject to the terms of this license, including the
//       license conditions and limitations in section 3, each contributor
//       grants you a non-exclusive, worldwide, royalty-free license under its
//       licensed patents to make, have made, use, sell, offer for sale,
//       import, and/or otherwise dispose of its contribution in the software
//       or derivative works of the contribution in the software.
// 
// 3. Conditions and Limitations
// 
//   (A) No Trademark License- This license does not grant you rights to use
//       any contributors' name, logo, or trademarks.
// 
//   (B) If you bring a patent claim against any contributor over patents that
//       you claim are infringed by the software, your patent license from such
//       contributor to the software ends automatically.
// 
//   (C) If you distribute any portion of the software, you must retain all
//       copyright, patent, trademark, and attribution notices that are present
//       in the software.
// 
//   (D) If you distribute any portion of the software in source code form, you
//       may do so only under this license by including a complete copy of this
//       license with your distribution. If you distribute any portion of the
//       software in compiled or object code form, you may only do so under a
//       license that complies with this license.
// 
//   (E) The software is licensed "as-is." You bear the risk of using it. The
//       contributors give no express warranties, guarantees or conditions. You
//       may have additional consumer rights under your local laws which this
//       license cannot change. To the extent permitted under your local laws,
//       the contributors exclude the implied warranties of merchantability,
//       fitness for a particular purpose and non-infringement.
//       

#include "ClearScriptV8Native.h"

//-----------------------------------------------------------------------------
// local helper functions
//-----------------------------------------------------------------------------

static HostObjectHolder* GetHostObjectHolder(v8::Local<v8::Object> hObject)
{
    _ASSERTE(hObject->InternalFieldCount() > 0);
    return static_cast<HostObjectHolder*>(hObject->GetAlignedPointerFromInternalField(0));
}

//-----------------------------------------------------------------------------

static void SetHostObjectHolder(v8::Local<v8::Object> hObject, HostObjectHolder* pHolder)
{
    _ASSERTE(hObject->InternalFieldCount() > 0);
    hObject->SetAlignedPointerInInternalField(0, pHolder);
}

//-----------------------------------------------------------------------------

static void* GetHostObject(v8::Local<v8::Object> hObject)
{
    auto pHolder = ::GetHostObjectHolder(hObject);
    return (pHolder != nullptr) ? pHolder->GetObject() : nullptr;
}

//-----------------------------------------------------------------------------

template<typename T>
static V8ContextImpl* UnwrapContextImplFromHolder(const v8::PropertyCallbackInfo<T>& info)
{
    auto hGlobal = info.Holder();
    _ASSERTE(hGlobal->InternalFieldCount() > 0);
    return static_cast<V8ContextImpl*>(hGlobal->GetAlignedPointerFromInternalField(0));
}

//-----------------------------------------------------------------------------

template <typename T>
static V8ContextImpl* UnwrapContextImplFromData(const v8::PropertyCallbackInfo<T>& info)
{
    return static_cast<V8ContextImpl*>(v8::Local<v8::External>::Cast(info.Data())->Value());
}

//-----------------------------------------------------------------------------

template <typename T>
static V8ContextImpl* UnwrapContextImplFromData(const v8::FunctionCallbackInfo<T>& info)
{
    return static_cast<V8ContextImpl*>(v8::Local<v8::External>::Cast(info.Data())->Value());
}

//-----------------------------------------------------------------------------

template <typename T>
static void* UnwrapHostObject(const v8::PropertyCallbackInfo<T>& info)
{
    return ::GetHostObject(info.Holder());
}

//-----------------------------------------------------------------------------

template <typename T>
static void* UnwrapHostObject(const v8::FunctionCallbackInfo<T>& info)
{
    return ::GetHostObject(info.Holder());
}

//-----------------------------------------------------------------------------

template <typename T>
static T CombineFlags(T flag1, T flag2)
{
    return static_cast<T>(static_cast<std::underlying_type_t<T>>(flag1) | static_cast<std::underlying_type_t<T>>(flag2));
}

//-----------------------------------------------------------------------------
// V8ContextImpl implementation
//-----------------------------------------------------------------------------

#define BEGIN_ISOLATE_SCOPE \
    { \
        V8IsolateImpl::Scope t_IsolateScope(m_spIsolateImpl);

#define END_ISOLATE_SCOPE \
        IGNORE_UNUSED(t_IsolateScope); \
    }

#define BEGIN_CONTEXT_SCOPE \
    { \
        Scope t_ContextScope(this);

#define END_CONTEXT_SCOPE \
        IGNORE_UNUSED(t_ContextScope); \
    }

#define BEGIN_EXECUTION_SCOPE \
    { \
        V8IsolateImpl::ExecutionScope t_IsolateExecutionScope(m_spIsolateImpl); \
        v8::TryCatch t_TryCatch;

#define END_EXECUTION_SCOPE \
        IGNORE_UNUSED(t_TryCatch); \
        IGNORE_UNUSED(t_IsolateExecutionScope); \
    }

#define VERIFY(RESULT) \
    Verify(t_TryCatch, RESULT)

#define VERIFY_CHECKPOINT() \
    Verify(t_TryCatch)

#define CALLBACK_RETURN(RESULT) \
    BEGIN_COMPOUND_MACRO \
        info.GetReturnValue().Set(RESULT); \
        return; \
    END_COMPOUND_MACRO

//-----------------------------------------------------------------------------

static std::atomic<size_t> s_InstanceCount(0);

//-----------------------------------------------------------------------------

V8ContextImpl::V8ContextImpl(V8IsolateImpl* pIsolateImpl, const StdString& name, bool enableDebugging, bool disableGlobalMembers, int debugPort):
    m_Name(name),
    m_spIsolateImpl(pIsolateImpl),
    m_AllowHostObjectConstructorCall(false),
    m_DisableHostObjectInterception(false)
{
    VerifyNotOutOfMemory();

    BEGIN_ISOLATE_SCOPE

        if (disableGlobalMembers)
        {
            m_hContext = CreatePersistent(CreateContext());
        }
        else
        {
            auto hGlobalTemplate = CreateObjectTemplate();
            hGlobalTemplate->SetInternalFieldCount(1);
            auto propertyHandlerFlags = ::CombineFlags(v8::PropertyHandlerFlags::kNonMasking, v8::PropertyHandlerFlags::kOnlyInterceptStrings);
            hGlobalTemplate->SetHandler(v8::NamedPropertyHandlerConfiguration(GetGlobalProperty, SetGlobalProperty, QueryGlobalProperty, DeleteGlobalProperty, GetGlobalPropertyNames, v8::Local<v8::Value>(), propertyHandlerFlags));
            hGlobalTemplate->SetHandler(v8::IndexedPropertyHandlerConfiguration(GetGlobalProperty, SetGlobalProperty, QueryGlobalProperty, DeleteGlobalProperty, GetGlobalPropertyIndices));

            m_hContext = CreatePersistent(CreateContext(nullptr, hGlobalTemplate));

            m_hGlobal = CreatePersistent(m_hContext->Global()->GetPrototype()->ToObject());
            _ASSERTE(m_hGlobal->InternalFieldCount() > 0);
            m_hGlobal->SetAlignedPointerInInternalField(0, this);
        }

        // Be careful when renaming the cookie or changing the way host objects are marked.
        // Such a change will require a corresponding change in the V8ScriptEngine constructor.

        m_hHostObjectCookieName = CreatePersistent(CreateString(StdString(L"{c2cf47d3-916b-4a3f-be2a-6ff567425808}")));
        m_hHostExceptionName = CreatePersistent(CreateString(StdString(L"hostException")));
        m_hAccessTokenName = CreatePersistent(CreateString(StdString(L"{cdc19e6e-5d80-4627-a605-bb4805f15086}")));

        m_hHostObjectTemplate = CreatePersistent(CreateFunctionTemplate());
        m_hHostObjectTemplate->SetClassName(CreateString(StdString(L"HostObject")));
        m_hHostObjectTemplate->SetCallHandler(HostObjectConstructorCallHandler, Wrap());
        m_hHostObjectTemplate->InstanceTemplate()->SetInternalFieldCount(1);
        m_hHostObjectTemplate->InstanceTemplate()->SetNamedPropertyHandler(GetHostObjectProperty, SetHostObjectProperty, QueryHostObjectProperty, DeleteHostObjectProperty, GetHostObjectPropertyNames, Wrap());
        m_hHostObjectTemplate->InstanceTemplate()->SetIndexedPropertyHandler(GetHostObjectProperty, SetHostObjectProperty, QueryHostObjectProperty, DeleteHostObjectProperty, GetHostObjectPropertyIndices, Wrap());
        m_hHostObjectTemplate->InstanceTemplate()->SetCallAsFunctionHandler(InvokeHostObject, Wrap());

        v8::Local<v8::Function> hToFunction;
        BEGIN_CONTEXT_SCOPE
            hToFunction = CreateFunction(CreateFunctionForHostDelegate, Wrap());
            m_hTerminationException = CreatePersistent(v8::Exception::Error(CreateString(StdString(L"Script execution was interrupted"))));
        END_CONTEXT_SCOPE

        m_hHostDelegateTemplate = CreatePersistent(CreateFunctionTemplate());
        m_hHostDelegateTemplate->SetClassName(CreateString(StdString(L"HostDelegate")));
        m_hHostDelegateTemplate->SetCallHandler(HostObjectConstructorCallHandler, Wrap());
        m_hHostDelegateTemplate->InstanceTemplate()->SetInternalFieldCount(1);
        m_hHostDelegateTemplate->InstanceTemplate()->SetNamedPropertyHandler(GetHostObjectProperty, SetHostObjectProperty, QueryHostObjectProperty, DeleteHostObjectProperty, GetHostObjectPropertyNames, Wrap());
        m_hHostDelegateTemplate->InstanceTemplate()->SetIndexedPropertyHandler(GetHostObjectProperty, SetHostObjectProperty, QueryHostObjectProperty, DeleteHostObjectProperty, GetHostObjectPropertyIndices, Wrap());
        m_hHostDelegateTemplate->InstanceTemplate()->SetCallAsFunctionHandler(InvokeHostObject, Wrap());
        m_hHostDelegateTemplate->PrototypeTemplate()->Set(CreateString(StdString(L"toFunction")), hToFunction);

        m_spIsolateImpl->AddContext(this, enableDebugging, debugPort);
        m_pvV8ObjectCache = HostObjectHelpers::CreateV8ObjectCache();

    END_ISOLATE_SCOPE

    ++s_InstanceCount;
}

//-----------------------------------------------------------------------------

size_t V8ContextImpl::GetInstanceCount()
{
    return s_InstanceCount;
}

//-----------------------------------------------------------------------------

size_t V8ContextImpl::GetMaxIsolateHeapSize()
{
    return m_spIsolateImpl->GetMaxHeapSize();
}

//-----------------------------------------------------------------------------

void V8ContextImpl::SetMaxIsolateHeapSize(size_t value)
{
    m_spIsolateImpl->SetMaxHeapSize(value);
}

//-----------------------------------------------------------------------------

double V8ContextImpl::GetIsolateHeapSizeSampleInterval()
{
    return m_spIsolateImpl->GetHeapSizeSampleInterval();
}

//-----------------------------------------------------------------------------

void V8ContextImpl::SetIsolateHeapSizeSampleInterval(double value)
{
    m_spIsolateImpl->SetHeapSizeSampleInterval(value);
}

//-----------------------------------------------------------------------------

size_t V8ContextImpl::GetMaxIsolateStackUsage()
{
    return m_spIsolateImpl->GetMaxStackUsage();
}

//-----------------------------------------------------------------------------

void V8ContextImpl::SetMaxIsolateStackUsage(size_t value)
{
    m_spIsolateImpl->SetMaxStackUsage(value);
}

//-----------------------------------------------------------------------------

void V8ContextImpl::CallWithLock(LockCallbackT* pCallback, void* pvArg)
{
    VerifyNotOutOfMemory();

    BEGIN_ISOLATE_SCOPE

        (*pCallback)(pvArg);

    END_ISOLATE_SCOPE
}

//-----------------------------------------------------------------------------

V8Value V8ContextImpl::GetRootObject()
{
    BEGIN_CONTEXT_SCOPE

        return ExportValue(m_hContext->Global());

    END_CONTEXT_SCOPE
}

//-----------------------------------------------------------------------------

void V8ContextImpl::SetGlobalProperty(const StdString& name, const V8Value& value, bool globalMembers)
{
    BEGIN_CONTEXT_SCOPE

        auto hName = CreateString(name);
        auto hValue = ImportValue(value);

        v8::Local<v8::Value> hOldValue;
        if (m_hContext->Global()->HasOwnProperty(hName))
        {
            hOldValue = m_hContext->Global()->GetRealNamedProperty(hName);
        }

        m_hContext->Global()->ForceSet(hName, hValue, (v8::PropertyAttribute)(v8::ReadOnly | v8::DontDelete));
        if (globalMembers && hValue->IsObject())
        {
            if (!hOldValue.IsEmpty() && hOldValue->IsObject())
            {
                for (auto it = m_GlobalMembersStack.begin(); it != m_GlobalMembersStack.end(); it++)
                {
                    if (it->first == name)
                    {
                        Dispose(it->second);
                        m_GlobalMembersStack.erase(it);
                        break;
                    }
                }
            }

            m_GlobalMembersStack.emplace_back(name, CreatePersistent(hValue->ToObject()));
        }

    END_CONTEXT_SCOPE
}

//-----------------------------------------------------------------------------

V8Value V8ContextImpl::Execute(const StdString& documentName, const StdString& code, bool evaluate, bool /*discard*/)
{
    BEGIN_CONTEXT_SCOPE
    BEGIN_EXECUTION_SCOPE

        v8::ScriptCompiler::Source source(CreateString(code), v8::ScriptOrigin(CreateString(documentName)));
        auto hScript = VERIFY(CreateScript(&source));
        auto hResult = VERIFY(hScript->Run());
        if (!evaluate)
        {
            hResult = GetUndefined();
        }

        return ExportValue(hResult);

    END_EXECUTION_SCOPE
    END_CONTEXT_SCOPE
}

//-----------------------------------------------------------------------------

V8ScriptHolder* V8ContextImpl::Compile(const StdString& documentName, const StdString& code)
{
    BEGIN_CONTEXT_SCOPE
    BEGIN_EXECUTION_SCOPE

        v8::ScriptCompiler::Source source(CreateString(code), v8::ScriptOrigin(CreateString(documentName)));
        auto hScript = VERIFY(CreateUnboundScript(&source));
        return new V8ScriptHolderImpl(GetWeakBinding(), ::PtrFromScriptHandle(CreatePersistent(hScript)));

    END_EXECUTION_SCOPE
    END_CONTEXT_SCOPE
}

//-----------------------------------------------------------------------------

bool V8ContextImpl::CanExecute(V8ScriptHolder* pHolder)
{
    return pHolder->IsSameIsolate(m_spIsolateImpl.GetRawPtr());
}

//-----------------------------------------------------------------------------

V8Value V8ContextImpl::Execute(V8ScriptHolder* pHolder, bool evaluate)
{
    BEGIN_CONTEXT_SCOPE
    BEGIN_EXECUTION_SCOPE

        auto hScript = ::ScriptHandleFromPtr(pHolder->GetScript());
        auto hResult = VERIFY(hScript->BindToCurrentContext()->Run());
        if (!evaluate)
        {
            hResult = GetUndefined();
        }

        return ExportValue(hResult);

    END_EXECUTION_SCOPE
    END_CONTEXT_SCOPE
}

//-----------------------------------------------------------------------------

void V8ContextImpl::Interrupt()
{
    TerminateExecution();
}

//-----------------------------------------------------------------------------

void V8ContextImpl::GetIsolateHeapInfo(V8IsolateHeapInfo& heapInfo)
{
    m_spIsolateImpl->GetHeapInfo(heapInfo);
}

//-----------------------------------------------------------------------------

void V8ContextImpl::CollectGarbage(bool exhaustive)
{
    m_spIsolateImpl->CollectGarbage(exhaustive);
}

//-----------------------------------------------------------------------------

void V8ContextImpl::OnAccessSettingsChanged()
{
    BEGIN_CONTEXT_SCOPE
    BEGIN_EXECUTION_SCOPE

        Dispose(m_hAccessToken);
        m_hAccessToken = CreatePersistent(CreateObject());

    END_EXECUTION_SCOPE
    END_CONTEXT_SCOPE
}

//-----------------------------------------------------------------------------

void V8ContextImpl::Destroy()
{
    m_spIsolateImpl->CallWithLockNoWait([this] (V8IsolateImpl* /*pIsolateImpl*/)
    {
        delete this;
    });
}

//-----------------------------------------------------------------------------

V8Value V8ContextImpl::GetV8ObjectProperty(void* pvObject, const StdString& name)
{
    BEGIN_CONTEXT_SCOPE
    BEGIN_EXECUTION_SCOPE

        return ExportValue(::ObjectHandleFromPtr(pvObject)->Get(CreateString(name)));

    END_EXECUTION_SCOPE
    END_CONTEXT_SCOPE
}

//-----------------------------------------------------------------------------

void V8ContextImpl::SetV8ObjectProperty(void* pvObject, const StdString& name, const V8Value& value)
{
    BEGIN_CONTEXT_SCOPE
    BEGIN_EXECUTION_SCOPE

        ::ObjectHandleFromPtr(pvObject)->Set(CreateString(name), ImportValue(value));

    END_EXECUTION_SCOPE
    END_CONTEXT_SCOPE
}

//-----------------------------------------------------------------------------

bool V8ContextImpl::DeleteV8ObjectProperty(void* pvObject, const StdString& name)
{
    BEGIN_CONTEXT_SCOPE

        return ::ObjectHandleFromPtr(pvObject)->Delete(CreateString(name));

    END_CONTEXT_SCOPE
}

//-----------------------------------------------------------------------------

void V8ContextImpl::GetV8ObjectPropertyNames(void* pvObject, std::vector<StdString>& names)
{
    BEGIN_CONTEXT_SCOPE

        GetV8ObjectPropertyNames(::ObjectHandleFromPtr(pvObject), names);

    END_CONTEXT_SCOPE
}

//-----------------------------------------------------------------------------

V8Value V8ContextImpl::GetV8ObjectProperty(void* pvObject, int index)
{
    BEGIN_CONTEXT_SCOPE
    BEGIN_EXECUTION_SCOPE

        return ExportValue(::ObjectHandleFromPtr(pvObject)->Get(index));

    END_EXECUTION_SCOPE
    END_CONTEXT_SCOPE
}

//-----------------------------------------------------------------------------

void V8ContextImpl::SetV8ObjectProperty(void* pvObject, int index, const V8Value& value)
{
    BEGIN_CONTEXT_SCOPE
    BEGIN_EXECUTION_SCOPE

        ::ObjectHandleFromPtr(pvObject)->Set(index, ImportValue(value));

    END_EXECUTION_SCOPE
    END_CONTEXT_SCOPE
}

//-----------------------------------------------------------------------------

bool V8ContextImpl::DeleteV8ObjectProperty(void* pvObject, int index)
{
    BEGIN_CONTEXT_SCOPE

        return ::ObjectHandleFromPtr(pvObject)->Delete(index);

    END_CONTEXT_SCOPE
}

//-----------------------------------------------------------------------------

void V8ContextImpl::GetV8ObjectPropertyIndices(void* pvObject, std::vector<int>& indices)
{
    BEGIN_CONTEXT_SCOPE

        GetV8ObjectPropertyIndices(::ObjectHandleFromPtr(pvObject), indices);

    END_CONTEXT_SCOPE
}

//-----------------------------------------------------------------------------

V8Value V8ContextImpl::InvokeV8Object(void* pvObject, const std::vector<V8Value>& args, bool asConstructor)
{
    BEGIN_CONTEXT_SCOPE
    BEGIN_EXECUTION_SCOPE

        auto hObject = ::ObjectHandleFromPtr(pvObject);

        std::vector<v8::Local<v8::Value>> importedArgs;
        ImportValues(args, importedArgs);

        if (asConstructor)
        {
            return ExportValue(VERIFY(hObject->CallAsConstructor(static_cast<int>(importedArgs.size()), importedArgs.data())));
        }

        return ExportValue(VERIFY(hObject->CallAsFunction(hObject, static_cast<int>(importedArgs.size()), importedArgs.data())));

    END_EXECUTION_SCOPE
    END_CONTEXT_SCOPE
}

//-----------------------------------------------------------------------------

V8Value V8ContextImpl::InvokeV8ObjectMethod(void* pvObject, const StdString& name, const std::vector<V8Value>& args)
{
    BEGIN_CONTEXT_SCOPE
    BEGIN_EXECUTION_SCOPE

        auto hObject = ::ObjectHandleFromPtr(pvObject);

        auto hName = CreateString(name);
        if (!hObject->Has(hName))
        {
            auto hError = v8::Exception::TypeError(CreateString(StdString(L"Method or property not found")))->ToObject();
            throw V8Exception(V8Exception::Type_General, m_Name, StdString(hError), StdString(hError->Get(CreateString(StdString(L"stack")))), V8Value(V8Value::Undefined));
        }

        auto hValue = hObject->Get(hName);
        if (hValue->IsUndefined() || hValue->IsNull())
        {
            auto hError = v8::Exception::TypeError(CreateString(StdString(L"Property value does not support invocation")))->ToObject();
            throw V8Exception(V8Exception::Type_General, m_Name, StdString(hError), StdString(hError->Get(CreateString(StdString(L"stack")))), V8Value(V8Value::Undefined));
        }

        std::vector<v8::Local<v8::Value>> importedArgs;
        ImportValues(args, importedArgs);

        auto hMethod = VERIFY(hValue->ToObject());
        return ExportValue(VERIFY(hMethod->CallAsFunction(hObject, static_cast<int>(importedArgs.size()), importedArgs.data())));

    END_EXECUTION_SCOPE
    END_CONTEXT_SCOPE
}

//-----------------------------------------------------------------------------

void V8ContextImpl::ProcessDebugMessages()
{
    BEGIN_CONTEXT_SCOPE

        v8::Debug::ProcessDebugMessages();

    END_CONTEXT_SCOPE
}

//-----------------------------------------------------------------------------

V8ContextImpl::~V8ContextImpl()
{
    _ASSERTE(m_spIsolateImpl->IsCurrent() && m_spIsolateImpl->IsLocked());
    --s_InstanceCount;

    std::vector<void*> v8ObjectPtrs;
    HostObjectHelpers::GetAllCachedV8Objects(m_pvV8ObjectCache, v8ObjectPtrs);
    for (auto pvV8Object: v8ObjectPtrs)
    {
        auto hObject = ::ObjectHandleFromPtr(pvV8Object);
        delete ::GetHostObjectHolder(hObject);
        ClearWeak(hObject);
        Dispose(hObject);
    }

    HostObjectHelpers::Release(m_pvV8ObjectCache);
    m_spIsolateImpl->RemoveContext(this);

    for (auto it = m_GlobalMembersStack.rbegin(); it != m_GlobalMembersStack.rend(); it++)
    {
        Dispose(it->second);
    }

    Dispose(m_hHostDelegateTemplate);
    Dispose(m_hHostObjectTemplate);
    Dispose(m_hAccessToken);
    Dispose(m_hAccessTokenName);
    Dispose(m_hHostExceptionName);
    Dispose(m_hHostObjectCookieName);

    // As of V8 3.16.0, the global property getter for a disposed context
    // may be invoked during GC after the V8ContextImpl instance is gone.

    if (!m_hGlobal.IsEmpty())
    {
        _ASSERTE(m_hGlobal->InternalFieldCount() > 0);
        m_hGlobal->SetAlignedPointerInInternalField(0, nullptr);
        Dispose(m_hGlobal);
    }

    Dispose(m_hContext);
    ContextDisposedNotification();
}

//-----------------------------------------------------------------------------

v8::Local<v8::Value> V8ContextImpl::Wrap()
{
    return CreateExternal(this);
}

//-----------------------------------------------------------------------------

SharedPtr<V8WeakContextBinding> V8ContextImpl::GetWeakBinding()
{
    if (m_spWeakBinding.IsEmpty())
    {
        m_spWeakBinding = new V8WeakContextBinding(m_spIsolateImpl, this);
    }

    return m_spWeakBinding;
}

//-----------------------------------------------------------------------------

bool V8ContextImpl::CheckContextImplForGlobalObjectCallback(V8ContextImpl* pContextImpl)
{
    if (pContextImpl == nullptr)
    {
        return false;
    }

    if (pContextImpl->IsExecutionTerminating())
    {
        pContextImpl->ThrowException(pContextImpl->m_hTerminationException);
        return false;
    }

    return true;
}

//-----------------------------------------------------------------------------

bool V8ContextImpl::CheckContextImplForHostObjectCallback(V8ContextImpl* pContextImpl)
{
    if (pContextImpl == nullptr)
    {
        return false;
    }

    if (pContextImpl->m_DisableHostObjectInterception)
    {
        return false;
    }

    if (pContextImpl->IsExecutionTerminating())
    {
        pContextImpl->ThrowException(pContextImpl->m_hTerminationException);
        return false;
    }

    return true;
}

//-----------------------------------------------------------------------------

void V8ContextImpl::GetV8ObjectPropertyNames(v8::Local<v8::Object> hObject, std::vector<StdString>& names)
{
    names.clear();

    auto hNames = hObject->GetPropertyNames();
    if (!hNames.IsEmpty())
    {
        auto nameCount = static_cast<int>(hNames->Length());

        names.reserve(nameCount);
        for (auto index = 0; index < nameCount; index++)
        {
            auto hName = hNames->Get(index);
            if (!hName.IsEmpty())
            {
                StdString name(hName);

                int propertyIndex;
                if (!HostObjectHelpers::TryParseInt32(name, propertyIndex))
                {
                    names.push_back(std::move(name));
                }
            }
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::GetV8ObjectPropertyIndices(v8::Local<v8::Object> hObject, std::vector<int>& indices)
{
    indices.clear();

    auto hNames = hObject->GetPropertyNames();
    if (!hNames.IsEmpty())
    {
        auto nameCount = static_cast<int>(hNames->Length());

        indices.reserve(nameCount);
        for (auto index = 0; index < nameCount; index++)
        {
            auto hName = hNames->Get(index);
            if (!hName.IsEmpty())
            {
                StdString name(hName);

                int propertyIndex;
                if (HostObjectHelpers::TryParseInt32(name, propertyIndex))
                {
                    indices.push_back(propertyIndex);
                }
            }
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::GetGlobalProperty(v8::Local<v8::Name> hName, const v8::PropertyCallbackInfo<v8::Value>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromHolder(info);
    if (CheckContextImplForGlobalObjectCallback(pContextImpl))
    {
        const auto& stack = pContextImpl->m_GlobalMembersStack;
        if (stack.size() > 0)
        {
            auto hKey = hName->ToString();
            if (!hKey->Equals(pContextImpl->m_hHostObjectCookieName))
            {
                for (auto it = stack.rbegin(); it != stack.rend(); it++)
                {
                    if (it->second->HasOwnProperty(hKey))
                    {
                        CALLBACK_RETURN(it->second->Get(hKey));
                    }
                }
            }
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::SetGlobalProperty(v8::Local<v8::Name> hName, v8::Local<v8::Value> hValue, const v8::PropertyCallbackInfo<v8::Value>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromHolder(info);
    if (CheckContextImplForGlobalObjectCallback(pContextImpl))
    {
        const auto& stack = pContextImpl->m_GlobalMembersStack;
        if (stack.size() > 0)
        {
            auto hKey = hName->ToString();
            if (!hKey->Equals(pContextImpl->m_hHostObjectCookieName))
            {
                for (auto it = stack.rbegin(); it != stack.rend(); it++)
                {
                    if (it->second->HasOwnProperty(hKey))
                    {
                        it->second->Set(hKey, hValue);
                        CALLBACK_RETURN(hValue);
                    }
                }
            }
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::QueryGlobalProperty(v8::Local<v8::Name> hName, const v8::PropertyCallbackInfo<v8::Integer>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromHolder(info);
    if (CheckContextImplForGlobalObjectCallback(pContextImpl))
    {
        const auto& stack = pContextImpl->m_GlobalMembersStack;
        if (stack.size() > 0)
        {
            auto hKey = hName->ToString();
            if (!hKey->Equals(pContextImpl->m_hHostObjectCookieName))
            {
                for (auto it = stack.rbegin(); it != stack.rend(); it++)
                {
                    if (it->second->HasOwnProperty(hKey))
                    {
                        CALLBACK_RETURN(it->second->GetPropertyAttributes(hKey));
                    }
                }
            }
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::DeleteGlobalProperty(v8::Local<v8::Name> hName, const v8::PropertyCallbackInfo<v8::Boolean>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromHolder(info);
    if (CheckContextImplForGlobalObjectCallback(pContextImpl))
    {
        const auto& stack = pContextImpl->m_GlobalMembersStack;
        if (stack.size() > 0)
        {
            auto hKey = hName->ToString();
            for (auto it = stack.rbegin(); it != stack.rend(); it++)
            {
                if (it->second->HasOwnProperty(hKey))
                {
                    // WORKAROUND: v8::Object::Delete() crashes if a custom property deleter calls
                    // ThrowException(). Interestingly, there is no crash if the same deleter is
                    // invoked directly from script via the delete operator.

                    if (it->second->HasOwnProperty(pContextImpl->m_hHostObjectCookieName))
                    {
                        try
                        {
                            CALLBACK_RETURN(HostObjectHelpers::DeleteProperty(::GetHostObject(it->second), StdString(hKey)));
                        }
                        catch (const HostException&)
                        {
                            CALLBACK_RETURN(false);
                        }
                    }

                    CALLBACK_RETURN(it->second->Delete(hKey));
                }
            }
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::GetGlobalPropertyNames(const v8::PropertyCallbackInfo<v8::Array>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromHolder(info);
    if (CheckContextImplForGlobalObjectCallback(pContextImpl))
    {
        try
        {
            const auto& stack = pContextImpl->m_GlobalMembersStack;
            if (stack.size() > 0)
            {
                std::vector<StdString> names;
                for (auto it = stack.rbegin(); it != stack.rend(); it++)
                {
                    std::vector<StdString> tempNames;
                    if (it->second->HasOwnProperty(pContextImpl->m_hHostObjectCookieName))
                    {
                        HostObjectHelpers::GetPropertyNames(::GetHostObject(it->second), tempNames);
                    }
                    else
                    {
                        pContextImpl->GetV8ObjectPropertyNames(it->second, tempNames);
                    }

                    names.insert(names.end(), tempNames.begin(), tempNames.end());
                }

                std::sort(names.begin(), names.end());
                auto newEnd = std::unique(names.begin(), names.end());
                auto nameCount = static_cast<int>(newEnd - names.begin());

                auto hImportedNames = pContextImpl->CreateArray(nameCount);
                for (auto index = 0; index < nameCount; index++)
                {
                    hImportedNames->Set(index, pContextImpl->CreateString(names[index]));
                }

                CALLBACK_RETURN(hImportedNames);
            }
        }
        catch (const HostException& exception)
        {
            pContextImpl->ThrowScriptException(exception);
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::GetGlobalProperty(unsigned __int32 index, const v8::PropertyCallbackInfo<v8::Value>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromHolder(info);
    if (CheckContextImplForGlobalObjectCallback(pContextImpl))
    {
        const auto& stack = pContextImpl->m_GlobalMembersStack;
        if (stack.size() > 0)
        {
            auto hName = pContextImpl->CreateInteger(index)->ToString();
            for (auto it = stack.rbegin(); it != stack.rend(); it++)
            {
                if (it->second->HasOwnProperty(hName))
                {
                    CALLBACK_RETURN(it->second->Get(index));
                }
            }
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::SetGlobalProperty(unsigned __int32 index, v8::Local<v8::Value> hValue, const v8::PropertyCallbackInfo<v8::Value>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromHolder(info);
    if (CheckContextImplForGlobalObjectCallback(pContextImpl))
    {
        const auto& stack = pContextImpl->m_GlobalMembersStack;
        if (stack.size() > 0)
        {
            auto hName = pContextImpl->CreateInteger(index)->ToString();
            for (auto it = stack.rbegin(); it != stack.rend(); it++)
            {
                if (it->second->HasOwnProperty(hName))
                {
                    it->second->Set(index, hValue);
                    CALLBACK_RETURN(hValue);
                }
            }
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::QueryGlobalProperty(unsigned __int32 index, const v8::PropertyCallbackInfo<v8::Integer>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromHolder(info);
    if (CheckContextImplForGlobalObjectCallback(pContextImpl))
    {
        const auto& stack = pContextImpl->m_GlobalMembersStack;
        if (stack.size() > 0)
        {
            auto hIndex = pContextImpl->CreateInteger(index);
            auto hName = hIndex->ToString();
            for (auto it = stack.rbegin(); it != stack.rend(); it++)
            {
                if (it->second->HasOwnProperty(hName))
                {
                    CALLBACK_RETURN(it->second->GetPropertyAttributes(hIndex));
                }
            }
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::DeleteGlobalProperty(unsigned __int32 index, const v8::PropertyCallbackInfo<v8::Boolean>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromHolder(info);
    if (CheckContextImplForGlobalObjectCallback(pContextImpl))
    {
        const auto& stack = pContextImpl->m_GlobalMembersStack;
        if (stack.size() > 0)
        {
            auto hName = pContextImpl->CreateInteger(index)->ToString();
            for (auto it = stack.rbegin(); it != stack.rend(); it++)
            {
                if (it->second->HasOwnProperty(hName))
                {
                    CALLBACK_RETURN(it->second->Delete(index));
                }
            }
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::GetGlobalPropertyIndices(const v8::PropertyCallbackInfo<v8::Array>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromHolder(info);
    if (CheckContextImplForGlobalObjectCallback(pContextImpl))
    {
        try
        {
            const auto& stack = pContextImpl->m_GlobalMembersStack;
            if (stack.size() > 0)
            {
                std::vector<int> indices;
                for (auto it = stack.rbegin(); it != stack.rend(); it++)
                {
                    std::vector<int> tempIndices;
                    if (it->second->HasOwnProperty(pContextImpl->m_hHostObjectCookieName))
                    {
                        HostObjectHelpers::GetPropertyIndices(::GetHostObject(it->second), tempIndices);
                    }
                    else
                    {
                        pContextImpl->GetV8ObjectPropertyIndices(it->second, tempIndices);
                    }

                    indices.insert(indices.end(), tempIndices.begin(), tempIndices.end());
                }

                std::sort(indices.begin(), indices.end());
                auto newEnd = std::unique(indices.begin(), indices.end());
                auto indexCount = static_cast<int>(newEnd - indices.begin());

                auto hImportedIndices = pContextImpl->CreateArray(indexCount);
                for (auto index = 0; index < indexCount; index++)
                {
                    hImportedIndices->Set(index, pContextImpl->CreateInteger(indices[index]));
                }

                CALLBACK_RETURN(hImportedIndices);
            }
        }
        catch (const HostException& exception)
        {
            pContextImpl->ThrowScriptException(exception);
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::HostObjectConstructorCallHandler(const v8::FunctionCallbackInfo<v8::Value>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromData(info);
    if ((pContextImpl != nullptr) && !pContextImpl->m_AllowHostObjectConstructorCall)
    {
        pContextImpl->ThrowException(v8::Exception::Error(pContextImpl->CreateString(StdString(L"This function is for ClearScript internal use only"))));
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::CreateFunctionForHostDelegate(const v8::FunctionCallbackInfo<v8::Value>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromData(info);
    if (pContextImpl != nullptr)
    {
        CALLBACK_RETURN(pContextImpl->CreateFunction(InvokeHostDelegate, info.This()));
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::InvokeHostDelegate(const v8::FunctionCallbackInfo<v8::Value>& info)
{
    auto hTarget = info.Data();

    auto argCount = info.Length();
    if (argCount < 1)
    {
        if (info.IsConstructCall())
        {
            CALLBACK_RETURN(hTarget->ToObject()->CallAsConstructor(0, nullptr));
        }

        CALLBACK_RETURN(hTarget->ToObject()->CallAsFunction(hTarget, 0, nullptr));
    }

    std::vector<v8::Local<v8::Value>> args;
    args.reserve(argCount);

    for (auto index = 0; index < argCount; index++)
    {
        args.push_back(info[index]);
    }

    if (info.IsConstructCall())
    {
        CALLBACK_RETURN(hTarget->ToObject()->CallAsConstructor(argCount, &args[0]));
    }

    CALLBACK_RETURN(hTarget->ToObject()->CallAsFunction(hTarget, argCount, &args[0]));
}

//-----------------------------------------------------------------------------

void V8ContextImpl::GetHostObjectProperty(v8::Local<v8::String> hName, const v8::PropertyCallbackInfo<v8::Value>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromData(info);
    if (CheckContextImplForHostObjectCallback(pContextImpl))
    {
        try
        {
            if (hName->Equals(pContextImpl->m_hHostObjectCookieName))
            {
                CALLBACK_RETURN(true);
            }

            auto hHolder = info.Holder();

            auto hAccessToken = hHolder->GetHiddenValue(pContextImpl->m_hAccessTokenName);
            if (pContextImpl->m_hAccessToken != hAccessToken)
            {
                BEGIN_PULSE_VALUE_SCOPE(&pContextImpl->m_DisableHostObjectInterception, true)

                    auto hNames = hHolder->GetOwnPropertyNames();
                    for (auto index = hNames->Length(); index > 0; index--)
                    {
                        hHolder->Delete(hNames->Get(index - 1));
                    }

                    hHolder->SetHiddenValue(pContextImpl->m_hAccessTokenName, pContextImpl->m_hAccessToken);

                END_PULSE_VALUE_SCOPE
            }

            auto hResult = hHolder->GetHiddenValue(hName);
            if (hResult.IsEmpty())
            {
                bool isCacheable;
                hResult = pContextImpl->ImportValue(HostObjectHelpers::GetProperty(::UnwrapHostObject(info), StdString(hName), isCacheable));
                if (isCacheable)
                {
                    hHolder->ForceSet(hName, hResult);
                }
            }

            CALLBACK_RETURN(hResult);
        }
        catch (const HostException& exception)
        {
            pContextImpl->ThrowScriptException(exception);
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::SetHostObjectProperty(v8::Local<v8::String> hName, v8::Local<v8::Value> hValue, const v8::PropertyCallbackInfo<v8::Value>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromData(info);
    if (CheckContextImplForHostObjectCallback(pContextImpl))
    {
        try
        {
            HostObjectHelpers::SetProperty(::UnwrapHostObject(info), StdString(hName), pContextImpl->ExportValue(hValue));
            CALLBACK_RETURN(hValue);
        }
        catch (const HostException& exception)
        {
            pContextImpl->ThrowScriptException(exception);
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::QueryHostObjectProperty(v8::Local<v8::String> hName, const v8::PropertyCallbackInfo<v8::Integer>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromData(info);
    if (CheckContextImplForHostObjectCallback(pContextImpl))
    {
        try
        {
            if (hName->Equals(pContextImpl->m_hHostObjectCookieName))
            {
                CALLBACK_RETURN(v8::ReadOnly | v8::DontEnum | v8::DontDelete);
            }

            std::vector<StdString> names;
            HostObjectHelpers::GetPropertyNames(::UnwrapHostObject(info), names);

            StdString name(hName);
            for (auto it = names.begin(); it != names.end(); it++)
            {
                if (it->Compare(name) == 0)
                {
                    CALLBACK_RETURN(v8::None);
                }
            }
        }
        catch (const HostException& exception)
        {
            pContextImpl->ThrowScriptException(exception);
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::DeleteHostObjectProperty(v8::Local<v8::String> hName, const v8::PropertyCallbackInfo<v8::Boolean>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromData(info);
    if (CheckContextImplForHostObjectCallback(pContextImpl))
    {
        try
        {
            CALLBACK_RETURN(HostObjectHelpers::DeleteProperty(::UnwrapHostObject(info), StdString(hName)));
        }
        catch (const HostException& exception)
        {
            pContextImpl->ThrowScriptException(exception);
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::GetHostObjectPropertyNames(const v8::PropertyCallbackInfo<v8::Array>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromData(info);
    if (CheckContextImplForHostObjectCallback(pContextImpl))
    {
        try
        {
            std::vector<StdString> names;
            HostObjectHelpers::GetPropertyNames(::UnwrapHostObject(info), names);
            auto nameCount = static_cast<int>(names.size());

            auto hImportedNames = pContextImpl->CreateArray(nameCount);
            for (auto index = 0; index < nameCount; index++)
            {
                hImportedNames->Set(index, pContextImpl->CreateString(names[index]));
            }

            CALLBACK_RETURN(hImportedNames);
        }
        catch (const HostException& exception)
        {
            pContextImpl->ThrowScriptException(exception);
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::GetHostObjectProperty(unsigned __int32 index, const v8::PropertyCallbackInfo<v8::Value>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromData(info);
    if (CheckContextImplForHostObjectCallback(pContextImpl))
    {
        try
        {
            CALLBACK_RETURN(pContextImpl->ImportValue(HostObjectHelpers::GetProperty(::UnwrapHostObject(info), index)));
        }
        catch (const HostException& exception)
        {
            pContextImpl->ThrowScriptException(exception);
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::SetHostObjectProperty(unsigned __int32 index, v8::Local<v8::Value> hValue, const v8::PropertyCallbackInfo<v8::Value>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromData(info);
    if (CheckContextImplForHostObjectCallback(pContextImpl))
    {
        try
        {
            HostObjectHelpers::SetProperty(::UnwrapHostObject(info), index, pContextImpl->ExportValue(hValue));
            CALLBACK_RETURN(hValue);
        }
        catch (const HostException& exception)
        {
            pContextImpl->ThrowScriptException(exception);
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::QueryHostObjectProperty(unsigned __int32 index, const v8::PropertyCallbackInfo<v8::Integer>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromData(info);
    if (CheckContextImplForHostObjectCallback(pContextImpl))
    {
        try
        {
            std::vector<int> indices;
            HostObjectHelpers::GetPropertyIndices(::UnwrapHostObject(info), indices);

            for (auto it = indices.begin(); it < indices.end(); it++)
            {
                if (*it == static_cast<int>(index))
                {
                    CALLBACK_RETURN(v8::None);
                }
            }
        }
        catch (const HostException& exception)
        {
            pContextImpl->ThrowScriptException(exception);
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::DeleteHostObjectProperty(unsigned __int32 index, const v8::PropertyCallbackInfo<v8::Boolean>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromData(info);
    if (CheckContextImplForHostObjectCallback(pContextImpl))
    {
        try
        {
            CALLBACK_RETURN(HostObjectHelpers::DeleteProperty(::UnwrapHostObject(info), index));
        }
        catch (const HostException& exception)
        {
            pContextImpl->ThrowScriptException(exception);
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::GetHostObjectPropertyIndices(const v8::PropertyCallbackInfo<v8::Array>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromData(info);
    if (CheckContextImplForHostObjectCallback(pContextImpl))
    {
        try
        {
            std::vector<int> indices;
            HostObjectHelpers::GetPropertyIndices(::UnwrapHostObject(info), indices);
            auto indexCount = static_cast<int>(indices.size());

            auto hImportedIndices = pContextImpl->CreateArray(indexCount);
            for (auto index = 0; index < indexCount; index++)
            {
                hImportedIndices->Set(index, pContextImpl->CreateInteger(indices[index]));
            }

            CALLBACK_RETURN(hImportedIndices);
        }
        catch (const HostException& exception)
        {
            pContextImpl->ThrowScriptException(exception);
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::InvokeHostObject(const v8::FunctionCallbackInfo<v8::Value>& info)
{
    auto pContextImpl = ::UnwrapContextImplFromData(info);
    if (CheckContextImplForHostObjectCallback(pContextImpl))
    {
        try
        {
            auto argCount = info.Length();

            std::vector<V8Value> exportedArgs;
            exportedArgs.reserve(argCount);

            for (auto index = 0; index < argCount; index++)
            {
                exportedArgs.push_back(pContextImpl->ExportValue(info[index]));
            }

            CALLBACK_RETURN(pContextImpl->ImportValue(HostObjectHelpers::Invoke(::UnwrapHostObject(info), exportedArgs, info.IsConstructCall())));
        }
        catch (const HostException& exception)
        {
            pContextImpl->ThrowScriptException(exception);
        }
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::DisposeWeakHandle(v8::Isolate* pIsolate, Persistent<v8::Object>* phObject, void* pvV8ObjectCache)
{
    IGNORE_UNUSED(pIsolate);

    auto pHolder = ::GetHostObjectHolder(*phObject);
    ASSERT_EVAL(HostObjectHelpers::RemoveV8ObjectCacheEntry(pvV8ObjectCache, pHolder->GetObject()));

    delete pHolder;
    phObject->Dispose();
}

//-----------------------------------------------------------------------------

v8::Local<v8::Value> V8ContextImpl::ImportValue(const V8Value& value)
{
    if (value.IsNonexistent())
    {
        return v8::Local<v8::Value>();
    }

    if (value.IsUndefined())
    {
        return GetUndefined();
    }

    if (value.IsNull())
    {
        return GetNull();
    }

    {
        bool result;
        if (value.AsBoolean(result))
        {
            return result ? GetTrue() : GetFalse();
        }
    }

    {
        double result;
        if (value.AsNumber(result))
        {
            return CreateNumber(result);
        }
    }

    {
        __int32 result;
        if (value.AsInt32(result))
        {
            return CreateInteger(result);
        }
    }

    {
        unsigned __int32 result;
        if (value.AsUInt32(result))
        {
            return CreateInteger(result);
        }
    }

    {
        const StdString* pString;
        if (value.AsString(pString))
        {
            return CreateString(*pString);
        }
    }

    {
        HostObjectHolder* pHolder;
        if (value.AsHostObject(pHolder))
        {
            auto pvV8Object = HostObjectHelpers::GetCachedV8Object(m_pvV8ObjectCache, pHolder->GetObject());
            if (pvV8Object != nullptr)
            {
                return CreateLocal(::ObjectHandleFromPtr(pvV8Object));
            }

            v8::Local<v8::Object> hObject;
            if (HostObjectHelpers::IsDelegate(pHolder->GetObject()))
            {
                BEGIN_PULSE_VALUE_SCOPE(&m_AllowHostObjectConstructorCall, true)
                    hObject = m_hHostDelegateTemplate->InstanceTemplate()->NewInstance();
                END_PULSE_VALUE_SCOPE
            }
            else
            {
                BEGIN_PULSE_VALUE_SCOPE(&m_AllowHostObjectConstructorCall, true)
                    hObject = m_hHostObjectTemplate->InstanceTemplate()->NewInstance();
                END_PULSE_VALUE_SCOPE
            }

            // WARNING: Instantiation may fail during script interruption. Check NewInstance()
            // result to avoid access violations and V8 fatal errors in ::SetObjectHolder().

            if (!hObject.IsEmpty())
            {
                ::SetHostObjectHolder(hObject, pHolder = pHolder->Clone());
                hObject->SetHiddenValue(m_hAccessTokenName, m_hAccessToken);
                pvV8Object = ::PtrFromObjectHandle(MakeWeak(CreatePersistent(hObject), m_pvV8ObjectCache, DisposeWeakHandle));
                HostObjectHelpers::CacheV8Object(m_pvV8ObjectCache, pHolder->GetObject(), pvV8Object);
            }

            return hObject;
        }
    }

    {
        V8ObjectHolder* pHolder;
        if (value.AsV8Object(pHolder))
        {
            return CreateLocal(::ObjectHandleFromPtr(pHolder->GetObject()));
        }
    }

    return GetUndefined();
}

//-----------------------------------------------------------------------------

V8Value V8ContextImpl::ExportValue(v8::Local<v8::Value> hValue)
{
    if (hValue.IsEmpty())
    {
        return V8Value(V8Value::Nonexistent);
    }

    if (hValue->IsUndefined())
    {
        return V8Value(V8Value::Undefined);
    }

    if (hValue->IsNull())
    {
        return V8Value(V8Value::Null);
    }

    if (hValue->IsBoolean())
    {
        return V8Value(hValue->BooleanValue());
    }

    if (hValue->IsNumber())
    {
        return V8Value(hValue->NumberValue());
    }

    if (hValue->IsInt32())
    {
        return V8Value(hValue->Int32Value());
    }

    if (hValue->IsUint32())
    {
        return V8Value(hValue->Uint32Value());
    }

    if (hValue->IsString())
    {
        return V8Value(new StdString(hValue));
    }

    if (hValue->IsObject())
    {
        auto hObject = hValue->ToObject();
        if (hObject->HasOwnProperty(m_hHostObjectCookieName))
        {
            return V8Value(::GetHostObjectHolder(hObject)->Clone());
        }

        return V8Value(new V8ObjectHolderImpl(GetWeakBinding(), ::PtrFromObjectHandle(CreatePersistent(hObject))));
    }

    return V8Value(V8Value::Undefined);
}

//-----------------------------------------------------------------------------

void V8ContextImpl::ImportValues(const std::vector<V8Value>& values, std::vector<v8::Local<v8::Value>>& importedValues)
{
    importedValues.clear();

    auto valueCount = static_cast<int>(values.size());
    importedValues.reserve(valueCount);

    for (auto index = 0; index < valueCount; index++)
    {
        importedValues.push_back(ImportValue(values[index]));
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::Verify(const v8::TryCatch& tryCatch)
{
    if (tryCatch.HasCaught())
    {
        if (!tryCatch.CanContinue())
        {
            VerifyNotOutOfMemory();
            throw V8Exception(V8Exception::Type_Interrupt, m_Name, StdString(L"Script execution interrupted by host"), StdString(tryCatch.StackTrace()), V8Value(V8Value::Undefined));
        }

        auto hException = tryCatch.Exception();
        if (hException->SameValue(m_hTerminationException))
        {
            VerifyNotOutOfMemory();
            throw V8Exception(V8Exception::Type_Interrupt, m_Name, StdString(L"Script execution interrupted by host"), StdString(tryCatch.StackTrace()), V8Value(V8Value::Undefined));
        }

        StdString message;
        bool stackOverflow;
        bool allowGenericError;

        StdString value(hException);
        if (value.GetLength() > 0)
        {
            message = std::move(value);
            stackOverflow = (_wcsicmp(message.ToCString(), L"RangeError: Maximum call stack size exceeded") == 0);
            allowGenericError = false;
        }
        else
        {
            // It is unclear why V8 sometimes generates blank exceptions, although it probably has
            // to do with memory pressure. It seems to happen only during stack overflow recovery.

            message = L"Unknown error; potential stack overflow detected";
            stackOverflow = true;
            allowGenericError = true;
        }

    #ifdef _DEBUG

        if (stackOverflow)
        {
            // Stack overflow conditions require extreme care, as V8's behavior can be erratic
            // until the stack is unwound a bit. Much of the code below can trigger unexpected
            // fatal errors in this context, so it makes sense to bypass it. On the other hand,
            // losing error information is also undesirable, and the detection code above is far
            // from robust. These sanity checks are intended to mitigate this fragility.

            _ASSERTE(hException->IsObject());
            StdString constructorName(hException->ToObject()->GetConstructorName());
            _ASSERTE((constructorName == L"RangeError") || (allowGenericError && (constructorName == L"Error")));
        }

    #endif // _DEBUG

        StdString stackTrace;
        V8Value hostException(V8Value::Undefined);

        if (stackOverflow)
        {
            stackTrace = message;
        }
        else
        {
            auto hStackTrace = tryCatch.StackTrace();
            if (!hStackTrace.IsEmpty())
            {
                stackTrace = StdString(hStackTrace);
            }

            auto hMessage = tryCatch.Message();
            if (!hMessage.IsEmpty())
            {
                if (message.GetLength() < 1)
                {
                    message = StdString(hMessage->Get());
                }

                stackTrace = message;

                auto hMessageStackTrace = hMessage->GetStackTrace();
                int frameCount = !hMessageStackTrace.IsEmpty() ? hMessageStackTrace->GetFrameCount() : 0;

                if (frameCount < 1)
                {
                    auto hScriptResourceName = hMessage->GetScriptResourceName();
                    if (!hScriptResourceName.IsEmpty() && !hScriptResourceName->IsNull() && !hScriptResourceName->IsUndefined())
                    {
                        auto hScriptName = hScriptResourceName->ToString();
                        if (!hScriptName.IsEmpty() && (hScriptName->Length() > 0))
                        {
                            stackTrace += L"\n    at ";
                            stackTrace += StdString(hScriptName);
                        }
                        else
                        {
                            stackTrace += L"\n    at <anonymous>";
                        }
                    }
                    else
                    {
                        stackTrace += L"\n    at <anonymous>";
                    }

                    stackTrace += L':';
                    stackTrace += std::to_wstring(hMessage->GetLineNumber());
                    stackTrace += L':';
                    stackTrace += std::to_wstring(hMessage->GetStartColumn() + 1);

                    auto hSourceLine = hMessage->GetSourceLine();
                    if (!hSourceLine.IsEmpty() && (hSourceLine->Length() > 0))
                    {
                        stackTrace += L" -> ";
                        stackTrace += StdString(hSourceLine);
                    }
                }
                else
                {
                    for (int index = 0; index < frameCount; index++)
                    {
                        auto hFrame = hMessageStackTrace->GetFrame(index);
                        stackTrace += L"\n    at ";

                        auto hFunctionName = hFrame->GetFunctionName();
                        if (!hFunctionName.IsEmpty() && (hFunctionName->Length() > 0))
                        {
                            if (hFrame->IsConstructor())
                            {
                                stackTrace += L"new ";
                            }

                            stackTrace += StdString(hFunctionName);
                            stackTrace += L" (";
                        }

                        auto hScriptName = hFrame->GetScriptName();
                        if (!hScriptName.IsEmpty() && (hScriptName->Length() > 0))
                        {
                            stackTrace += StdString(hScriptName);
                        }
                        else
                        {
                            stackTrace += L"<anonymous>";
                        }

                        stackTrace += L':';
                        auto lineNumber = hFrame->GetLineNumber();
                        if (lineNumber != v8::Message::kNoLineNumberInfo)
                        {
                            stackTrace += std::to_wstring(lineNumber);
                        }

                        stackTrace += L':';
                        auto column = hFrame->GetColumn();
                        if (column != v8::Message::kNoColumnInfo)
                        {
                            stackTrace += std::to_wstring(column);
                        }

                        if (!hFunctionName.IsEmpty() && (hFunctionName->Length() > 0))
                        {
                            stackTrace += L')';
                        }

                        if (index == 0)
                        {
                            auto hSourceLine = hMessage->GetSourceLine();
                            if (!hSourceLine.IsEmpty() && (hSourceLine->Length() > 0))
                            {
                                stackTrace += L" -> ";
                                stackTrace += StdString(hSourceLine);
                            }
                        }
                    }
                }
            }

            if (hException->IsObject())
            {
                hostException = ExportValue(hException->ToObject()->Get(m_hHostExceptionName));
            }
        }

        throw V8Exception(V8Exception::Type_General, m_Name, std::move(message), std::move(stackTrace), std::move(hostException));
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::VerifyNotOutOfMemory()
{
    if (m_spIsolateImpl->IsOutOfMemory())
    {
        m_spIsolateImpl->ThrowOutOfMemoryException();
    }
}

//-----------------------------------------------------------------------------

void V8ContextImpl::ThrowScriptException(const HostException& exception)
{
    auto hException = v8::Exception::Error(CreateString(exception.GetMessage()))->ToObject();

    auto hHostException = ImportValue(exception.GetException());
    if (!hHostException.IsEmpty() && hHostException->IsObject())
    {
        hException->Set(m_hHostExceptionName, hHostException);
    }

    ThrowException(hException);
}

//
// $safeitemname$.xaml.cpp
// Implementation of the $safeitemname$ class
//

#include "pch.h"
#include "$safeitemname$.xaml.h"

using namespace $rootnamespace$;

using namespace Platform;
using namespace Platform::Collections;
using namespace concurrency;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

// The Grouped Items Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234231

$wizardregistrationcomment$$safeitemname$::$safeitemname$()
{
	InitializeComponent();
	SetValue(_defaultViewModelProperty, ref new Map<String^,Object^>(std::less<String^>()));
	auto navigationHelper = ref new Common::NavigationHelper(this);
	SetValue(_navigationHelperProperty, navigationHelper);
	navigationHelper->LoadState += ref new Common::LoadStateEventHandler(this, &$safeitemname$::LoadState);
}

DependencyProperty^ $safeitemname$::_navigationHelperProperty = nullptr;
DependencyProperty^ $safeitemname$::_defaultViewModelProperty = nullptr;

void $safeitemname$::RegisterDependencyProperties()
{
	if (_navigationHelperProperty == nullptr)
	{
		_navigationHelperProperty = DependencyProperty::Register("NavigationHelper",
			TypeName(Common::NavigationHelper::typeid), TypeName($safeitemname$::typeid), nullptr);
	}

	if (_defaultViewModelProperty == nullptr)
	{
		_defaultViewModelProperty =
			DependencyProperty::Register("DefaultViewModel",
				TypeName(IObservableMap<String^, Object^>::typeid), TypeName($safeitemname$::typeid), nullptr);
	}
}

/// <summary>
/// used as a trivial view model.
/// </summary>
IObservableMap<String^, Object^>^ $safeitemname$::DefaultViewModel::get()
{
	return safe_cast<IObservableMap<String^, Object^>^>(GetValue(_defaultViewModelProperty));
}

/// <summary>
/// Gets an implementation of <see cref="NavigationHelper"/> designed to be
/// used as a trivial view model.
/// </summary>
Common::NavigationHelper^ $safeitemname$::NavigationHelper::get()
{
	return safe_cast<Common::NavigationHelper^>(GetValue(_navigationHelperProperty));
}

#pragma region Navigation support

/// The methods provided in this section are simply used to allow
/// NavigationHelper to respond to the page's navigation methods.
/// 
/// Page specific logic should be placed in event handlers for the  
/// <see cref="NavigationHelper::LoadState"/>
/// and <see cref="NavigationHelper::SaveState"/>.
/// The navigation parameter is available in the LoadState method 
/// in addition to page state preserved during an earlier session.

void $safeitemname$::OnNavigatedTo(NavigationEventArgs^ e)
{
	this->NavigationHelper->OnNavigatedTo(e);
}

void $safeitemname$::OnNavigatedFrom(NavigationEventArgs^ e)
{
	this->NavigationHelper->OnNavigatedFrom(e);
}

#pragma endregion

/// <summary>
/// 
/// </summary>
void $safeitemname$::LoadState(Object^ sender, Common::LoadStateEventArgs^ e)
{
	(void) sender;	// Unused parameter
	(void) e;	// Unused parameter

	// TODO: Set a collection of bindable groups using DefaultViewModel->Insert("Groups", <value>)
}

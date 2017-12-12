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

// The Item Detail Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234232

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
	NavigationHelper->OnNavigatedTo(e);
}

void $safeitemname$::OnNavigatedFrom(NavigationEventArgs^ e)
{
	NavigationHelper->OnNavigatedFrom(e);
}

#pragma endregion

/// <summary>
/// Populates the page with content passed during navigation.  Any saved state is also
/// provided when recreating a page from a prior session.
/// </summary>
/// <param name="navigationParameter">The parameter value passed to
/// <see cref="Frame::Navigate(Type, Object)"/> when this page was initially requested.
/// </param>
/// <param name="pageState">A map of state preserved by this page during an earlier
/// session.  This will be null the first time a page is visited.</param>
void $safeitemname$::LoadState(Object^ sender, Common::LoadStateEventArgs^ e)
{
	String^ navigationParameter = safe_cast<String^>(e->NavigationParameter);

	// Allow saved page state to override the initial item to display
	if (e->PageState != nullptr && e->PageState->HasKey("SelectedItem"))
	{
		navigationParameter = safe_cast<String^>(e->PageState->Lookup("SelectedItem"));
	}

	// TODO: Set a bindable group using DefaultViewModel->Insert("Group", <value>)
	// TODO: Set a collection of bindable items using DefaultViewModel->Insert("Items", <value>)
	// TODO: Assign the selected item to flipView->SelectedItem
}

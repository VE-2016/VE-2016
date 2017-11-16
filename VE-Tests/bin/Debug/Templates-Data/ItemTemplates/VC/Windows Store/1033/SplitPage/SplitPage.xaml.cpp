//
// $safeitemname$.xaml.cpp
// Implementation of the $safeitemname$ class
//

#include "pch.h"
#include "$safeitemname$.xaml.h"

using namespace $rootnamespace$;
using namespace $rootnamespace$::Common;

using namespace Platform;
using namespace Platform::Collections;
using namespace concurrency;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

// The Split Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234234

$wizardregistrationcomment$$safeitemname$::$safeitemname$()
{
	InitializeComponent();
	SetValue(_defaultViewModelProperty, ref new Map<String^,Object^>(std::less<String^>()));
	auto navigationHelper = ref new Common::NavigationHelper(this,
		ref new Common::RelayCommand(
		[this](Object^) -> bool
	{
		return CanGoBack();
	},
		[this](Object^) -> void
	{
		GoBack();
	}
	)
		);
	SetValue(_navigationHelperProperty, navigationHelper);
	navigationHelper->LoadState += ref new Common::LoadStateEventHandler(this, &$safeitemname$::LoadState);
	navigationHelper->SaveState += ref new Common::SaveStateEventHandler(this, &$safeitemname$::SaveState);

	itemListView->SelectionChanged += ref new SelectionChangedEventHandler(this, &$safeitemname$::ItemListView_SelectionChanged);
	Window::Current->SizeChanged += ref new WindowSizeChangedEventHandler (this, &$safeitemname$::Window_SizeChanged);
	InvalidateVisualState();

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
	//	return _navigationHelper;
	return safe_cast<Common::NavigationHelper^>(GetValue(_navigationHelperProperty));
}

#pragma region Page state management

/// <summary>
/// Populates the page with content passed during navigation.  Any saved state is also
/// provided when recreating a page from a prior session.
/// </summary>
/// <param name="navigationParameter">The parameter value passed to
/// <see cref="Frame::Navigate(Type, Object)"/> when this page was initially requested.
/// </param>
/// <param name="pageState">A map of state preserved by this page during an earlier
/// session.  This will be null the first time a page is visited.</param>
void $safeitemname$::LoadState(Platform::Object^ sender, Common::LoadStateEventArgs^ e)
{
	// TODO: Set a bindable group using DefaultViewModel->Insert("Group", <value>)
	// TODO: Set a collection of bindable items using DefaultViewModel->Insert("Items", <value>)

	if (e->PageState == nullptr)
	{
		// When this is a new page, select the first item automatically unless logical page
		// navigation is being used (see the logical page navigation #region below.)
		if (!UsingLogicalPageNavigation() && itemsViewSource->View != nullptr)
		{
			itemsViewSource->View->MoveCurrentToFirst();
		}
	}
	else
	{
		// Restore the previously saved state associated with this page
		if (e->PageState->HasKey("SelectedItem") && itemsViewSource->View != nullptr)
		{
			// TODO: Invoke itemsViewSource->View->MoveCurrentTo() with the selected
			//       item as specified by the value of pageState->Lookup("SelectedItem")
		}
	}
}

/// <summary>
/// Preserves state associated with this page in case the application is suspended or the
/// page is discarded from the navigation cache.  Values must conform to the serialization
/// requirements of <see cref="SuspensionManager::SessionState"/>.
/// </summary>
/// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
/// <param name="e">Event data that provides an empty dictionary to be populated with
/// serializable state.</param>
void $safeitemname$::SaveState(Platform::Object^ sender, Common::SaveStateEventArgs^ e)
{
	if (itemsViewSource->View != nullptr)
	{
		auto selectedItem = itemsViewSource->View->CurrentItem;
		// TODO: Derive a serializable navigation parameter and pass it to
		//       pageState->Insert("SelectedItem", <value>)
	}
}

#pragma endregion

#pragma region Logical page navigation

// Visual state management typically reflects the four application view states directly (full
// screen landscape and portrait plus snapped and filled views.)  The split page is designed so
// that the snapped and portrait view states each have two distinct sub-states: either the item
// list or the details are displayed, but not both at the same time.
//
// This is all implemented with a single physical page that can represent two logical pages.
// The code below achieves this goal without making the user aware of the distinction.

/// <summary>
/// Invoked to determine whether the page should act as one logical page or two.
/// </summary>
/// <returns>True when the current view state is portrait or snapped, false
/// otherwise.</returns>
bool $safeitemname$::CanGoBack()
{
	if (UsingLogicalPageNavigation() && itemListView->SelectedItem != nullptr)
	{
		return true;
	}
	else
	{
		return NavigationHelper->CanGoBack();
	}
}

void $safeitemname$::GoBack()
{
	if (UsingLogicalPageNavigation() && itemListView->SelectedItem != nullptr)
	{
		// When logical page navigation is in effect and there's a selected item that
		// item's details are currently displayed.  Clearing the selection will return to
		// the item list.  From the user's point of view this is a logical backward
		// navigation.
		itemListView->SelectedItem = nullptr;
	}
	else
	{
		NavigationHelper->GoBack();
	}
}

/// <summary>
/// Invoked with the Window changes size
/// </summary>
/// <param name="sender">The current Window</param>
/// <param name="e">Event data that describes the new size of the Window</param>
void $safeitemname$::Window_SizeChanged(Platform::Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ e)
{
	InvalidateVisualState();
}

/// <summary>
/// Invoked when an item within the list is selected.
/// </summary>
/// <param name="sender">The GridView displaying the selected item.</param>
/// <param name="e">Event data that describes how the selection was changed.</param>
void $safeitemname$::ItemListView_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
	if (UsingLogicalPageNavigation())
	{
		InvalidateVisualState();
	}
}

/// <summary>
/// Invoked to determine whether the page should act as one logical page or two.
/// </summary>
/// <returns>True if the window should show act as one logical page, false
/// otherwise.</returns>
bool $safeitemname$::UsingLogicalPageNavigation()
{
	return Windows::UI::Xaml::Window::Current->Bounds.Width < MinimumWidthForSupportingTwoPanes;
}

void $safeitemname$::InvalidateVisualState()
{
	auto visualState = DetermineVisualState();
	Windows::UI::Xaml::VisualStateManager::GoToState(this, visualState, false);
	NavigationHelper->GoBackCommand->RaiseCanExecuteChanged();
}

/// <summary>
/// Invoked to determine the name of the visual state that corresponds to an application
/// view state.
/// </summary>
/// <returns>The name of the desired visual state.  This is the same as the name of the
/// view state except when there is a selected item in portrait and snapped views where
/// this additional logical page is represented by adding a suffix of _Detail.</returns>
Platform::String^ $safeitemname$::DetermineVisualState()
{
	if (!UsingLogicalPageNavigation())
		return "PrimaryView";

	// Update the back button's enabled state when the view state changes
	auto logicalPageBack = UsingLogicalPageNavigation() && itemListView->SelectedItem != nullptr;

	return logicalPageBack ? "SinglePane_Detail" : "SinglePane";
}

#pragma endregion

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
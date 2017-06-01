//
// $safeitemname$.xaml.cpp
// Implementation of the $safeitemname$ class.
//

#include "pch.h"
#include <collection.h>
#include "$safeitemname$.xaml.h"

using namespace $rootnamespace$;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::ApplicationModel::Activation;
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

// TODO: Connect the Search Results Page to your in-app search.
// The Search Results Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234240

$wizardregistrationcomment$$safeitemname$::$safeitemname$()
{
	InitializeComponent();

	SetValue(_defaultViewModelProperty, ref new Map<String^,Object^>(std::less<String^>()));
	SetValue(_navigationHelperProperty, ref new Common::NavigationHelper(this));

	NavigationHelper->LoadState += ref new Common::LoadStateEventHandler(this, &$safeitemname$::LoadState);
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
	(void) sender;	// Unused parameter

	// Unpack the two values passed in the parameter object: query text and previous Window content
	auto queryText = safe_cast<String^>(e->NavigationParameter);

	// TODO: Application-specific searching logic.  The search process is responsible for
	//       creating a list of user-selectable result categories:
	//
	//       filterList->Append(ref new $safeitemname$Filter("<filter name>", <result count>), false);
	//
	//       Only the first filter, typically "All", should pass true as a third argument in
	//       order to start in an active state.  Results for the active filter are provided
	//       in Filter_SelectionChanged below.

	auto filterList = ref new Vector<Object^>();
	filterList->Append(ref new $safeitemname$Filter("All", 0, true));

	// Communicate results through the view model
	DefaultViewModel->Insert("QueryText", "\u201c" + queryText + "\u201d");
	DefaultViewModel->Insert("Filters", filterList);
	DefaultViewModel->Insert("ShowFilters", filterList->Size > 1);
}

/// <summary>
/// Invoked when a filter is selected using a RadioButton when not snapped.
/// </summary>
/// <param name="sender">The selected RadioButton instance.</param>
/// <param name="e">Event data describing how the RadioButton was selected.</param>
void $safeitemname$::Filter_Checked(Object^ sender, RoutedEventArgs^ e)
{
	(void) e;	// Unused parameter
	auto filter = dynamic_cast<FrameworkElement^>(sender)->DataContext;

	// Mirror the change into the CollectionViewSource.
	// This is most likely not needed.
	if (filtersViewSource->View != nullptr)
	{
		filtersViewSource->View->MoveCurrentTo(filter);
	}

	// Determine what filter was selected
	auto selectedFilter = dynamic_cast<$safeitemname$Filter^>(filter);
	if (selectedFilter != nullptr)
	{
		// Mirror the results into the corresponding filter object to allow the
		// RadioButton representation used when not snapped to reflect the change
		selectedFilter->Active = true;

		// TODO: Respond to the change in active filter by calling DefaultViewModel->Insert("Results", <value>)
		//       where <value> is a collection of items with bindable Image, Title, Subtitle, and Description properties

		// Ensure results are found
		IVector<Object^>^ resultsCollection;
		if (this->DefaultViewModel->HasKey("Results") == true)
		{
			resultsCollection = dynamic_cast<IVector<Object^>^>(this->DefaultViewModel->Lookup("Results"));
			if (resultsCollection != nullptr && resultsCollection->Size != 0)
			{
				VisualStateManager::GoToState(this, "ResultsFound", true);
				return;
			}
		}
	}

	// Display informational text when there are no search results.
	VisualStateManager::GoToState(this, "NoResultsFound", true);
}

$safeitemname$Filter::$safeitemname$Filter(String^ name, int count, bool active): _count(0), _active(false)
{
	Name = name;
	Count = count;
	Active = active;
}

String^ $safeitemname$Filter::Name::get()
{
	return _name;
}

void $safeitemname$Filter::Name::set(String^ value)
{
	if (value == _name || (value != nullptr && value->Equals(_name)))
	{
		return;
	}

	_name = value;
	OnPropertyChanged("Name");
	OnPropertyChanged("Description");
}

int $safeitemname$Filter::Count::get()
{
	return _count;
}

void $safeitemname$Filter::Count::set(int value)
{
	if (value == _count)
	{
		return;
	}

	_count = value;
	OnPropertyChanged("Count");
	OnPropertyChanged("Description");
}

bool $safeitemname$Filter::Active::get()
{
	return _active;
}

void $safeitemname$Filter::Active::set(bool value)
{
	if (value == _active)
	{
		return;
	}
	_active = value; OnPropertyChanged("Active");
}

String^ $safeitemname$Filter::Description::get()
{
	return _name + " (" + _count.ToString() + ")";
}

void $safeitemname$Filter::OnPropertyChanged(String^ propertyName)
{
	PropertyChanged(this, ref new PropertyChangedEventArgs(propertyName));
}
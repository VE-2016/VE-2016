//
// $safeitemname$.xaml.cpp
// Implementation of the $safeitemname$ class
//

#include "pch.h"
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
using namespace Windows::Storage::Pickers::Provider;

// The File Open Picker Contract item template is documented at https://go.microsoft.com/fwlink/?LinkId=234239

$wizardcomment$$wizardregistrationcomment$$safeitemname$::$safeitemname$()
{
	InitializeComponent();
	SetValue(_defaultViewModelProperty, ref new Map<String^,Object^>(std::less<String^>()));
	Window::Current->SizeChanged += ref new WindowSizeChangedEventHandler (this, &$safeitemname$::Window_SizeChanged);
	InvalidateVisualState();
}

DependencyProperty^ $safeitemname$::_defaultViewModelProperty = nullptr;

void $safeitemname$::RegisterDependencyProperties()
{
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
/// Invoked when another application wants to open files from this application.
/// </summary>
/// <param name="e">Activation data used to coordinate the process with Windows.</param>
void $safeitemname$::Activate(FileOpenPickerActivatedEventArgs^ e)
{
	_fileOpenPickerUI = e->FileOpenPickerUI;
	_fileOpenPickerUI->FileRemoved += 
		ref new TypedEventHandler<FileOpenPickerUI^,FileRemovedEventArgs^> (this,&$safeitemname$::FilePickerUI_FileRemoved);
		
	// TODO: Use DefaultViewModel->Insert("Files", <value>) where <value> is a collection
	//     of items, each of which should have bindable Image, Title, and Description

	DefaultViewModel->Insert("CanGoUp", false);
	Window::Current->Content = this;
	Window::Current->Activate();
}

void $safeitemname$::Window_SizeChanged(Platform::Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ e)
{
	InvalidateVisualState();
}

void $safeitemname$::InvalidateVisualState()
{
	auto visualState = DetermineVisualState();
	Windows::UI::Xaml::VisualStateManager::GoToState(this, visualState, false);
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
		return Windows::UI::Xaml::Window::Current->Bounds.Width >= 500 ? "HorizontalView" : "VerticalView";
}

/// <summary>
/// Invoked when user removes one of the items from the Picker basket
/// </summary>
/// <param name="sender">The FileOpenPickerUI instance used to contain the available files.</param>
/// <param name="e">Event data that describes the file removed.</param>
void $safeitemname$::FilePickerUI_FileRemoved(FileOpenPickerUI^ sender, FileRemovedEventArgs^ e)
{
	// TODO: Respond to an item being deselected in the picker UI.
}

/// <summary>
/// Invoked when the selected collection of files changes.
/// </summary>
/// <param name="sender">The GridView instance used to display the available files.</param>
/// <param name="e">Event data that describes how the selection changed.</param>
void $safeitemname$::FileGridView_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
	(void) sender;	// Unused parameter
	(void) e;	// Unused parameter

	// TODO: Update Windows UI using _fileOpenPickerUI->AddFile and _fileOpenPickerUI->RemoveFile
}

/// <summary>
/// Invoked when the "Go up" button is clicked, indicating that the user wants to pop up
/// a level in the hierarchy of files.
/// </summary>
/// <param name="sender">The Button instance used to represent the "Go up" command.</param>
/// <param name="e">Event data that describes how the button was clicked.</param>
void $safeitemname$::GoUpButton_Click(Object^ sender, RoutedEventArgs^ e)
{
	(void) sender;	// Unused parameter
	(void) e;	// Unused parameter

	// TODO: Use DefaultViewModel->Insert("CanGoUp", true) to enable the corresponding command,
	//       reflect file hierarchy traversal with DefaultViewModel->Insert("Files", <value>)
}


// 
// $safeitemname$.xaml.h
// Declaration of the $safeitemname$ class
//

#pragma once


#include "$wizarditemsubpath$$safeitemname$.g.h"

namespace $rootnamespace$
{
	/// <summary>
	/// This page displays files owned by the application so that the user can grant another application
	/// access to them.
	/// </summary>
	public ref class $safeitemname$ sealed
	{
	public:
		$safeitemname$();

		static void RegisterDependencyProperties();
		static property Windows::UI::Xaml::DependencyProperty^ DefaultViewModelProperty
		{
			Windows::UI::Xaml::DependencyProperty^ get() { return _defaultViewModelProperty; }
		}

		/// <summary>
		/// This can be changed to a strongly typed view model.
		/// </summary>
		property Windows::Foundation::Collections::IObservableMap<Platform::String^, Platform::Object^>^ DefaultViewModel
		{
			Windows::Foundation::Collections::IObservableMap<Platform::String^, Platform::Object^>^  get();
		}

		void Activate(Windows::ApplicationModel::Activation::FileOpenPickerActivatedEventArgs^ e);

	private:
		Windows::Storage::Pickers::Provider::FileOpenPickerUI^ _fileOpenPickerUI;
		void FileGridView_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
		void GoUpButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void FilePickerUI_FileRemoved(Windows::Storage::Pickers::Provider::FileOpenPickerUI^ sender, Windows::Storage::Pickers::Provider::FileRemovedEventArgs^ e);
		void Window_SizeChanged(Platform::Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ e);
		void InvalidateVisualState();
		Platform::String^ DetermineVisualState();

		static Windows::UI::Xaml::DependencyProperty^ _defaultViewModelProperty;
	};
}

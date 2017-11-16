//
// $safeitemname$.xaml.h
// Declaration of the $safeitemname$ class.
//

#pragma once

#include "Common\BooleanToVisibilityConverter.h" // Required by generated header
#include "Common\SuspensionManager.h" //Required for Activation
#include "Common\NavigationHelper.h"
#include "$safeitemname$.g.h"

namespace $rootnamespace$
{
	/// <summary>
	/// This page displays search results when a global search is directed to this application.
	/// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class $safeitemname$ sealed
	{
	public:
		$safeitemname$();

		static void RegisterDependencyProperties();
		static property Windows::UI::Xaml::DependencyProperty^ DefaultViewModelProperty
		{
			Windows::UI::Xaml::DependencyProperty^ get() { return _defaultViewModelProperty; }
		}

		static property Windows::UI::Xaml::DependencyProperty^ NavigationHelperProperty
		{
			Windows::UI::Xaml::DependencyProperty^ get() { return _navigationHelperProperty; }
		}

		/// <summary>
		/// This can be changed to a strongly typed view model.
		/// </summary>
		property Windows::Foundation::Collections::IObservableMap<Platform::String^, Platform::Object^>^ DefaultViewModel
		{
			Windows::Foundation::Collections::IObservableMap<Platform::String^, Platform::Object^>^  get();
		}

		/// <summary>
		/// NavigationHelper is used on each page to aid in navigation and 
		/// process lifetime management
		/// </summary>
		property Common::NavigationHelper^ NavigationHelper
		{
			Common::NavigationHelper^ get();
		}

	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
		virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

	private:
		void LoadState(Platform::Object^ sender, Common::LoadStateEventArgs^ e);
		void Filter_Checked(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

		static Windows::UI::Xaml::DependencyProperty^ _defaultViewModelProperty;
		static Windows::UI::Xaml::DependencyProperty^ _navigationHelperProperty;
	};

	/// <summary>
	/// View model describing one of the filters available for viewing search results.
	/// </summary>
	[Windows::UI::Xaml::Data::Bindable]
	public ref class $safeitemname$Filter sealed : Windows::UI::Xaml::Data::INotifyPropertyChanged
	{
	private:
		Platform::String^ _name;
		int _count;
		bool _active;

	public:
		$safeitemname$Filter(Platform::String^ name, int count, bool active);

		virtual event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged;

		property Platform::String^ Name
		{
			Platform::String^ get();
			void set(Platform::String^ value);
		};

		property int Count
		{
			int get();
			void set(int value);
		};

		property bool Active
		{
			bool get();
			void set(bool value);
		};

		property Platform::String^ Description
		{
			Platform::String^ get();
		};

	protected:
		virtual void OnPropertyChanged(Platform::String^ propertyName);
	};
}

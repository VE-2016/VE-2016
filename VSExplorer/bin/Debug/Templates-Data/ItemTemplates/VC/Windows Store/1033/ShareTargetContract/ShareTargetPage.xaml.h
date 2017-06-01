//
// $safeitemname$.xaml.h
// Declaration of the $safeitemname$ class.
//

#pragma once

#include "Common\BooleanToVisibilityConverter.h" // Required by generated header
#include "Common\BooleanNegationConverter.h" // Required by generated header
#include "$safeitemname$.g.h"

#include <agile.h>

namespace $rootnamespace$
{
	/// <summary>
	/// This page allows other applications to share content through this application.
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

		void Activate(Windows::ApplicationModel::Activation::ShareTargetActivatedEventArgs^ e);

	private:
		Platform::Agile<Windows::ApplicationModel::DataTransfer::ShareTarget::ShareOperation> _shareOperation;
		void ShareButton_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

		static Windows::UI::Xaml::DependencyProperty^ _defaultViewModelProperty;

	};
}

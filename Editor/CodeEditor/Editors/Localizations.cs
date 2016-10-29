
using System;
using System.Collections.Generic;
using System.Text;

namespace AIMS.Libraries.CodeEditor.WinForms
{
    public sealed class Localizations
    {
        private static string s_findDialogText = "Find";
        private static string s_findNextButtonText = "Next Next";
        private static string s_findReplaceButtonText = "Replace";
        private static string s_findMarkAllButtonText = "Mark All";
        private static string s_findCloseButtonText = "Close";
        private static string s_findWhatLabelText = "Find What:";
        private static string s_findReplaceWithLabelText = "Replace With:";
        private static string s_findMatchCaseLabel = "Match Case";
        private static string s_findMatchWholeWordLabel = "Match whole word";
        private static string s_findUseRegExLabel = "Use regular expressions";
        private static string s_replaceDialogText = "Replace";
        private static string s_findReplaceAllButtonText = "Replace All";

        public static string FindReplaceAllButtonText
        {
            get { return Localizations.s_findReplaceAllButtonText; }
            set { Localizations.s_findReplaceAllButtonText = value; }
        }

        public static string ReplaceDialogText
        {
            get { return Localizations.s_replaceDialogText; }
            set { Localizations.s_replaceDialogText = value; }
        }

        public static string FindUseRegExLabel
        {
            get { return Localizations.s_findUseRegExLabel; }
            set { Localizations.s_findUseRegExLabel = value; }
        }

        public static string FindMatchWholeWordLabel
        {
            get { return Localizations.s_findMatchWholeWordLabel; }
            set { Localizations.s_findMatchWholeWordLabel = value; }
        }

        public static string FindMatchCaseLabel
        {
            get { return Localizations.s_findMatchCaseLabel; }
            set { Localizations.s_findMatchCaseLabel = value; }
        }

        public static string FindReplaceWithLabelText
        {
            get { return Localizations.s_findReplaceWithLabelText; }
            set { Localizations.s_findReplaceWithLabelText = value; }
        }

        public static string FindWhatLabelText
        {
            get { return Localizations.s_findWhatLabelText; }
            set { Localizations.s_findWhatLabelText = value; }
        }

        public static string FindCloseButtonText
        {
            get { return Localizations.s_findCloseButtonText; }
            set { Localizations.s_findCloseButtonText = value; }
        }

        public static string FindMarkAllButtonText
        {
            get { return Localizations.s_findMarkAllButtonText; }
            set { Localizations.s_findMarkAllButtonText = value; }
        }

        public static string FindReplaceButtonText
        {
            get { return Localizations.s_findReplaceButtonText; }
            set { Localizations.s_findReplaceButtonText = value; }
        }

        public static string FindNextButtonText
        {
            get { return Localizations.s_findNextButtonText; }
            set { Localizations.s_findNextButtonText = value; }
        }

        public static string FindDialogText
        {
            get { return Localizations.s_findDialogText; }
            set { Localizations.s_findDialogText = value; }
        }
    }
}

namespace Microsoft.VisualStudio.Text.Editor
{
// oe NOTICE using the original-Gtk-editor.
	internal interface IMdTextView : ITextView
    {
        MonoDevelop.SourceEditor.IMDSpaceReservationManager GetSpaceReservationManager(string name);

        Gtk.Container VisualElement
        {
            get;
        }

        void Focus();
	}
}

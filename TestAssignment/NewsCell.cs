using Foundation;
using System;
using UIKit;

namespace TestAssignment
{
    public partial class NewsCell : UITableViewCell
    {

        public NewsCell(IntPtr handle) : base(handle)
        {
        }

        public void UpdateCell(News ns)
        {
            TitleLabel.Text = ns.title;
            ContentLabel.Text = ns.contents;

            var link = ns.url;
            var htmlLink = String.Format("<a href='{0}'>{1}</a>", link, link);

            var attr = new NSAttributedStringDocumentAttributes()
            {
                DocumentType = NSDocumentType.HTML
            };

            var nsError = new NSError();
            UrlTextView.ShouldInteractWithUrl += delegate
            {
                return true;
            };
            UrlTextView.Editable = false;
            UrlTextView.AttributedText = new NSAttributedString(htmlLink, attr, ref nsError);
            UrlTextView.UserInteractionEnabled = true;
        }
    }
}
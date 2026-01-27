namespace CodeIn.Web.Components;

public static class ComponentTargets
{
    public const string AnchorButton = nameof(AnchorButton);
    public const string ActionArea = nameof(ActionArea);
    public const string ActionFile = nameof(ActionFile);
    public const string SocialGroup = nameof(SocialGroup);
    public const string FloatingFooter = nameof(FloatingFooter);
    public const string Highlight = nameof(Highlight);
    public const string ActionSelect = nameof(ActionSelect);
    public const string MarkdownBlock = nameof(MarkdownBlock);
    public const string Badge = nameof(Badge);
    public const string Icon = nameof(Icon);
    public const string Logo = nameof(Logo);
    public const string IconButton = nameof(IconButton);
    public const string JobItem = nameof(JobItem);

    public static class Accordion
    {
        public const string Base = nameof(Accordion);
        public const string Item = Base + nameof(Item);
        public const string Trigger = Base + nameof(Trigger);
        public const string Content = Base + nameof(Content);
    }

    public static class ActionInput
    {
        public const string Base = nameof(ActionInput);
        public const string Error = Base + nameof(Error);
    }
    
    public static class Avatar
    {
        public const string Base = nameof(Avatar);
        public const string Image = Base + nameof(Image);
        public const string Fallback = Base + nameof(Fallback);
    }

    public static class Company
    {
        private const string Base = nameof(Company);
        public const string Card = Base + nameof(Card);
        public const string Header = Base + nameof(Header);
        public const string Table = Base + nameof(Table);
    }

    public static class Card
    {
        public const string Base = nameof(Card);
        public const string Content = Base + nameof(Content);
        public const string Title = Base + nameof(Title);
        public const string Description = Base + nameof(Description);
        public const string Header = Base + nameof(Header);
        public const string Footer = Base + nameof(Footer);
    }

    public static class Modal
    {
        public const string Base = nameof(Modal);
        public const string Content = Base + nameof(Content);
        public const string Title = Base + nameof(Title);
        public const string Description = Base + nameof(Description);
        public const string Header = Base + nameof(Header);
        public const string Footer = Base + nameof(Footer);
        public const string Action = Base + nameof(Action);
        public const string Cancel = Base + nameof(Cancel);
    }
}

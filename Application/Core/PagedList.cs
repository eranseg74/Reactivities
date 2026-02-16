namespace Application.Core;

// The TCursor defines the event by which the application will fetch the next group of items.
// For example, for the activities, since we order them by their creation data it is reasonable to defined the cursor by date. The cursor refers to the item from which the next block of items will start
public class PagedList<T, TCursor>
{
    public List<T> Items { get; set; } = [];
    public TCursor? NextCursor { get; set; }
}

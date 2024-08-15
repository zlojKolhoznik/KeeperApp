using CommunityToolkit.Mvvm.ComponentModel;
using FuzzySharp;
using KeeperApp.Records;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace KeeperApp.Models
{
    public delegate void StructureChangedEventHandler(Folder newParent, Record record);

    public class RecordTree : ObservableObject
    {
        public event StructureChangedEventHandler StructureChanged;
        private ObservableCollection<RecordTreeItem> rootItems;

        public RecordTree(IEnumerable<Record> source, bool ignoreParents = false)
        {
            RootItems = ConvertToTree(source, ignoreParents);
        }

        public ObservableCollection<RecordTreeItem> RootItems
        {
            get => rootItems; 
            set => SetProperty(ref rootItems, value);
        }

        public ObservableCollection<Record> SearchByTitle(string title)
        {
            return new ObservableCollection<Record>(SearchByTitle(RootItems, title).Select(i => i.Record));
        }

        private ObservableCollection<RecordTreeItem> SearchByTitle(ObservableCollection<RecordTreeItem> root, string title)
        {
            var result = new ObservableCollection<RecordTreeItem>();
            foreach (var item in root)
            {
                double ratio = Fuzz.PartialRatio(item.Record.Title, title) / 100.0;
                ratio *= Fuzz.PartialTokenSortRatio(item.Record.Title, title) / 100.0;
                if (ratio > .6)
                {
                    result.Add(item);
                }
                if (item.Children is not null)
                {
                    var children = SearchByTitle(item.Children, title);
                    foreach (var child in children)
                    {
                        result.Add(child);
                    }
                }
            }
            return result;
        }

        private ObservableCollection<RecordTreeItem> ConvertToTree(IEnumerable<Record> source, bool ignoreParents)
        {
            var result = new ObservableCollection<RecordTreeItem>(source.Where(r => r.ParentId is null || ignoreParents)
                .Select(r => new RecordTreeItem 
                {
                    Record = r,
                    Children = r is Folder folder ? PopulateChildren(folder, source) : null
                }));
            result.CollectionChanged += (s, e) =>
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (RecordTreeItem item in e.NewItems)
                    {
                        StructureChanged?.Invoke(null, item.Record);
                    }
                }
            };
            return result;
        }

        private ObservableCollection<RecordTreeItem> PopulateChildren(Folder parent, IEnumerable<Record> source)
        {
            var children = new ObservableCollection<RecordTreeItem>(source.Where(r => r.ParentId == parent.Id)
                .Select(r => new RecordTreeItem
                {
                    Record = r,
                    Children = r is Folder folder ? PopulateChildren(folder, source) : null
                }));
            children.CollectionChanged += (s, e) =>
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (RecordTreeItem item in e.NewItems)
                    {
                        StructureChanged?.Invoke(parent, item.Record);
                    }
                }
            };
            return children;
        }

        public RecordTreeItem Find(Record record)
        {
            return Find(RootItems, record);
        }

        private RecordTreeItem Find(ObservableCollection<RecordTreeItem> source, Record record)
        {
            RecordTreeItem result = null;
            foreach (var item in source)
            {
                if (item.Record == record)
                {
                    result = item;
                }
                else if (item.Children is not null)
                {
                    var found = Find(item.Children, record);
                    if (found is not null)
                    {
                        result = found;
                    }
                }
            }
            return result;
        }

        public void Insert(Record record)
        {
            if (record.ParentId is null)
            {
                Insert(null, record);
            }
            else
            {
                foreach (var item in RootItems)
                {
                    if (Insert(item, record))
                    {
                        break;
                    }
                }
            }
        }

        private bool Insert(RecordTreeItem parent, Record record)
        {
            bool result = false;
            if (parent is null)
            {
                RootItems.Add(new RecordTreeItem
                {
                    Record = record
                });
                result = true;
            }
            else if (record.ParentId == parent.Record.Id)
            {
                parent.Children.Add(new RecordTreeItem
                {
                    Record = record
                });
                result = true;
            }
            else if (parent.Children is not null)
            {
                foreach (var item in parent.Children)
                {
                    result = Insert(item, record);
                    break;
                }
            }
            return result;
        }

        public void Remove(Record record)
        {
            Remove(RootItems, record);
        }

        private void Remove(ObservableCollection<RecordTreeItem> source, Record record)
        {
            foreach (var item in source)
            {
                if (item.Record == record)
                {
                    source.Remove(item);
                    break;
                }
                else if (item.Children is not null)
                {
                    Remove(item.Children, record);
                }
            }
        }

        public void UpdateLocation(Record record)
        {
            var item = Find(record);
            var oldParent = FindItemParent(item);
            var newContainer = FindRecordContainer(record);
            (oldParent?.Children ?? RootItems).Remove(item);
            newContainer.Add(item);
        }

        private RecordTreeItem FindItemParent(RecordTreeItem item)
        {
            return FindItemParent(RootItems, item);
        }

        private RecordTreeItem FindItemParent(ObservableCollection<RecordTreeItem> source, RecordTreeItem item)
        {
            RecordTreeItem result = null;
            foreach (var potentialParent in source)
            {
                if (potentialParent?.Children?.Contains(item) ?? false)
                {
                    result = potentialParent;
                }
                else if (potentialParent.Children is not null)
                {
                    var parent = FindItemParent(potentialParent.Children, potentialParent);
                    if (parent is not null)
                    {
                        result = parent;
                    }
                }
            }
            return result;
        }

        private ObservableCollection<RecordTreeItem> FindRecordContainer(Record record)
        {
            return FindRecordContainer(RootItems, record) ?? RootItems;
        }

        private ObservableCollection<RecordTreeItem> FindRecordContainer(ObservableCollection<RecordTreeItem> source, Record record)
        {
            ObservableCollection<RecordTreeItem> result = source.FirstOrDefault(i => i.Record.Id == record.ParentId)?.Children;
            if (result is null)
            {
                foreach (var item in source)
                {
                    if (item.Children is not null)
                    {
                        result = FindRecordContainer(item.Children, record);
                        if (result is not null)
                        {
                            break;
                        }
                    }
                }
            }
            return result;
        }
    }
}

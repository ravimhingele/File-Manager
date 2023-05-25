using System;
//using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FMg
{
    internal class ListViewColumnComparer : IComparer
    {
        public int ColumnIndex { get; set; }

        public ListViewColumnComparer(int columnIndex)
        {
            ColumnIndex = columnIndex;
        }

        public int Compare(object x, object y)
        {
            try
            {
                if (ColumnIndex == 4)
                {
                    ListViewItem X = (ListViewItem)x;
                    ListViewItem Y = (ListViewItem)y;

                    string[] Months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                    string[] p1 = X.SubItems[ColumnIndex].Text.Split(' ');
                    string[] p2 = Y.SubItems[ColumnIndex].Text.Split(' ');
                    if (p1[2] == p2[2])
                    {
                        if (p1[0] == p2[0])
                        {
                            int day1 = Convert.ToInt16(p1[1].Substring(0, p1[1].Length - 1));
                            int day2 = Convert.ToInt16(p2[1].Substring(0, p2[1].Length - 1));
                            return day1.CompareTo(day2);
                        }
                        else
                        {
                            return Array.IndexOf(Months, p1[0]).CompareTo(Array.IndexOf(Months, p2[0]));
                        }
                    }
                    else
                    {
                        return p2[2].CompareTo(p1[2]);
                    }
                }
                return String.Compare(((ListViewItem)x).SubItems[ColumnIndex].Text, ((ListViewItem)y).SubItems[ColumnIndex].Text);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}

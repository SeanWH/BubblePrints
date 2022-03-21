﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlueprintExplorer
{
    public partial class CtrlP : Form
    {
        public CtrlP()
        {
            InitializeComponent();
            Form1.InstallReadline(input);
            input.KeyDown += Input_KeyDown;
            root.KeyDown += Input_KeyDown;
            input.TextChanged += Input_TextChanged;

            root.ReadOnly = true;
            root.Cursor = Cursors.Arrow;

            root.AutoGenerateColumns = false;
            root.Columns.Add("Name", "Name");
            root.Columns.Add("Type", "Type");
            root.Columns.Add("Guid", "Guid");
            root.Columns[0].Width = 800;
            root.Columns[0].DataPropertyName = "Name";
            root.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            root.Columns[1].Width = 600;
            root.Columns[1].DataPropertyName = "Type";

            root.Columns[2].Width = 450;
            root.Columns[2].DataPropertyName = "GuidText";

            root.RowHeadersVisible = false;
            root.ColumnHeadersVisible = false;
            root.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            root.MultiSelect = false;
            root.AllowUserToResizeRows = false;

            if (Form1.Dark)
            {
                BubbleTheme.DarkenControls(input, root);
                BubbleTheme.DarkenStyles(root.DefaultCellStyle, root.ColumnHeadersDefaultCellStyle);
            }

            root.MouseClick += Root_MouseClick;

            DoubleBuffered = true;

        }

        private void Root_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle || e.Button == MouseButtons.Left)
            {
                var row = root.HitTest(e.X, e.Y).RowIndex;
                if (row >= 0 && row < root.Rows.Count)
                {
                    root.Rows[row].Selected = true;
                    Daddy.ShowBlueprint(row, ModifierKeys.HasFlag(Keys.Control) || e.Button == MouseButtons.Middle);
                    Close();
                }
            }
            Console.WriteLine(e.Button);
        }

        public Form1 Daddy;

        private void Input_TextChanged(object sender, EventArgs e)
        {
            if (input.Text.Length > 0)
            {
                Daddy.InvalidateResults(input.Text);
            }
        }

        private void UpdateSize()
        {
            int neededHeight = root.Rows.GetRowsHeight(DataGridViewElementStates.None);
            if (neededHeight < 640)
            {
                Height = neededHeight + 82;
            }
            else
            {
                Height = 640 + 82;
            }
        }

        public void SetResults(List<BlueprintHandle> results)
        {
            root.DataSource = results;
            UpdateSize();
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
            if (e.KeyCode == Keys.Up || (e.KeyCode == Keys.P && ModifierKeys.HasFlag(Keys.Control)))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                int current = root.SelectedRow();
                if (current > 0)
                {
                    root.Rows[current - 1].Selected = true;
                    root.CurrentCell = root.Rows[current - 1].Cells[0];
                }
            }
            if (e.KeyCode == Keys.Down || (e.KeyCode == Keys.N && ModifierKeys.HasFlag(Keys.Control)))
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                int current = root.SelectedRow();
                if (current < (root.Rows.Count - 1))
                {
                    root.Rows[current + 1].Selected = true;
                    root.CurrentCell = root.Rows[current + 1].Cells[0];
                }
            }

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {

                e.Handled = true;
                e.SuppressKeyPress = true;

                Daddy.ShowBlueprint(root.SelectedRow(), ModifierKeys.HasFlag(Keys.Control));

                Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            savedVerticalScroll = root.FirstDisplayedScrollingRowIndex;
        }

        private int savedVerticalScroll = -1;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            UpdateSize();
            if (savedVerticalScroll != -1)
            {
                root.FirstDisplayedScrollingRowIndex = savedVerticalScroll;
            }
            input.Focus();
        }
    }
}

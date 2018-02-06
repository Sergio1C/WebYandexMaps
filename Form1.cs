using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace WebYandexMaps
{
    public partial class Form1 : Form
    {

        public List<AddressDataSource> addressList;

        public Form1()
        {
            InitializeComponent();
            
            //copy "index.html" file in current directory
            string htmlPath = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName;
            string htmlText = File.ReadAllText(htmlPath + "\\index.html");
            webBrowser.DocumentText = htmlText;
        
            addressList = new List<AddressDataSource>();
            for (int i = 0; i < 150; i++)
            {
                addressList.Add(new AddressDataSource());
            }

            dataGridView.DataSource = addressList;

        }

        //dataGridView
        private void UpdateGeoObjects(int index = -1)
        {
            foreach (AddressDataSource it in addressList)
            {
                int indexOfList = addressList.IndexOf(it);
                if (index >= 0 && indexOfList != index) continue;

                webBrowser.Document.InvokeScript("AddGeoObject", new object[] { index, it.Address });
            }

        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (addressList == null) return;

            object valueCell = dataGridView.CurrentCell.Value;
            string addressText = (valueCell == null) ? "" : valueCell.ToString(); //пустое значение при очистке

            addressList[dataGridView.CurrentRow.Index].Address = addressText;
            UpdateGeoObjects(dataGridView.CurrentRow.Index);
        }

        private void dataGridView_KeyUp(object sender, KeyEventArgs e)
        {
            //paste from clipboard
            if ((e.Shift && e.KeyCode == Keys.Insert) || (e.Control && e.KeyCode == Keys.V))
            {
                char[] rowSplitter = { '\r', '\n' };
                char[] columnSplitter = { '\t' };

                IDataObject dataInClipboard = Clipboard.GetDataObject();

                string stringInClipboard = (string)dataInClipboard.GetData(DataFormats.UnicodeText);
                //split it into lines
                string[] rowsInClipboard = stringInClipboard.Split(rowSplitter, StringSplitOptions.RemoveEmptyEntries);

                //get the row and column of selected cell in grid
                int r = dataGridView.SelectedCells[0].RowIndex;
                int c = dataGridView.SelectedCells[0].ColumnIndex;

                //add rows into grid to fit clipboard lines
                if (dataGridView.Rows.Count < (r + rowsInClipboard.Length))
                {
                    dataGridView.Rows.Add(r + rowsInClipboard.Length - dataGridView.Rows.Count);
                }
                // loop through the lines, split them into cells and place the values in the corresponding cell.
                for (int iRow = 0; iRow < rowsInClipboard.Length; iRow++)
                {
                    string[] valuesInRow = rowsInClipboard[iRow].Split(columnSplitter);
                    for (int iCol = 0; iCol < valuesInRow.Length; iCol++)
                    {
                        if (dataGridView.ColumnCount - 1 >= c + iCol)
                        {
                            dataGridView.Rows[r + iRow].Cells[c + iCol].Value = valuesInRow[iCol];
                        }
                        UpdateGeoObjects(iRow);
                    }

                }
            }
            //delete selected rows
            if (e.KeyCode == Keys.Delete)
            {
                for (int iRow = 0; iRow < dataGridView.SelectedCells.Count; iRow++)
                {
                    DataGridViewCell cell = dataGridView.SelectedCells[iRow];
                    cell.Value = string.Empty;
                    UpdateGeoObjects(cell.RowIndex);
                }
            }
        }

        private void dataGridView_SelectionChanged(object sender, System.EventArgs e)
        {
            int index = dataGridView.CurrentRow.Index;
            webBrowser.Document.InvokeScript("UpdateIcon", new object[] { index });
        }

        //webBrowser
        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {            
            if(e.Url.Host.Contains("api-maps.yandex.ru"))
            {
                UpdateGeoObjects();
            }
        }
       
    }
}


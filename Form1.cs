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
            for (int i = 0; i < 20; i++)
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


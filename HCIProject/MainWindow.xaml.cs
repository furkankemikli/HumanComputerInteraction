using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;
using System.IO;
using System.Windows.Markup;

namespace HCIProject
{
    public class EmailItem
    {
        public string senderAddress { get; set; }
        public List<Recepient> recipients { get; set; }
        public string title { get; set; }
        public TextRange messageBody { get; set; }
        public List<string> attachments { get; set; }
        public int id;
        public string type;
    }

    public class Recepient
    {
        public string address { get; set; }
        public string role { get; set; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<EmailItem> allMails = new List<EmailItem>();

        public static mailSystemUC myController;

        public static int generalId = 0;

        public static SendMail sendMail = null;

        public MainWindow()
        {
            Loaded += MainWindow_Load;
            InitializeComponent();
        }

        private void Menu_ContextMenuClosing_1(object sender, ContextMenuEventArgs e)
        {

        }

        private void btnReply_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void MainWindow_Load(object sender, RoutedEventArgs e) {
            string path = "sources\\source.xml";
            takeFromXml(path);
            folderClicked();
        }

        private void writeToXml(string path) {
            
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            XmlWriter writer = XmlWriter.Create(path,settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("maillist");
            for (int i = 0; i < allMails.Count; i++)
            {
                writer.WriteStartElement("email");
                writer.WriteStartElement("sender");
                writer.WriteElementString("senderaddress", allMails[i].senderAddress);
                writer.WriteEndElement();
                writer.WriteStartElement("recipients");
                for (int j = 0; j < allMails[i].recipients.Count; j++)
                {
                    writer.WriteStartElement("recipient");
                    writer.WriteElementString("recipientaddress", allMails[i].recipients[j].address);
                    writer.WriteElementString("role", allMails[i].recipients[j].role);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteElementString("type", allMails[i].type);
                writer.WriteElementString("emailtitle", allMails[i].title);
                writer.WriteElementString("messagebody", GetXaml(allMails[i].messageBody));
                writer.WriteStartElement("attachments");
                for (int j = 0; j < allMails[i].attachments.Count; j++)
                {
                    writer.WriteStartElement("attachment");
                    writer.WriteElementString("filepath", allMails[i].attachments[j]);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.Close();
        }

        static string GetXaml(TextRange range)
        {
            MemoryStream stream = new MemoryStream();
            range.Save(stream, DataFormats.Xaml);
            string xamlText = Encoding.UTF8.GetString(stream.ToArray());
            return xamlText;
        }

        static TextRange SetXaml(string xamlString)
        {
            RichTextBox richText = new RichTextBox();
            StringReader stringReader = new StringReader(xamlString);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            Section sec = XamlReader.Load(xmlReader) as Section;
            FlowDocument doc = new FlowDocument();
            while (sec.Blocks.Count > 0)
                doc.Blocks.Add(sec.Blocks.FirstBlock);
            richText.Document = doc;
            return new TextRange(richText.Document.ContentStart, richText.Document.ContentEnd);
        }

        private void takeFromXml(string path) {
            XmlReader reader = null;
            try
            {
                reader = XmlReader.Create(path);
           
            txtMail.Text = "";
            int size;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "senderaddress") {
                        reader.Read();
                        allMails.Add(new EmailItem());
                        allMails.ElementAt(allMails.Count - 1).id = generalId;
                        generalId = generalId + 1;
                        allMails.ElementAt(allMails.Count - 1).recipients = new List<Recepient>();
                        allMails.ElementAt(allMails.Count - 1).attachments = new List<string>();
                        allMails.ElementAt(allMails.Count - 1).senderAddress = reader.Value;
                    }
                    else if (reader.Name == "type") {
                        reader.Read();
                        allMails.ElementAt(allMails.Count - 1).type = reader.Value;
                    }
                    else if (reader.Name == "recipientaddress")
                    {
                        reader.Read();
                        allMails.ElementAt(allMails.Count - 1).recipients.Add(new Recepient());
                        size = allMails.ElementAt(allMails.Count - 1).recipients.Count;
                        allMails.ElementAt(allMails.Count - 1).recipients.ElementAt(size - 1).address = reader.Value;

                    }
                    else if (reader.Name == "role")
                    {

                        reader.Read();
                        size = allMails.ElementAt(allMails.Count - 1).recipients.Count;
                        allMails.ElementAt(allMails.Count - 1).recipients.ElementAt(size - 1).role = reader.Value;

                    }
                    else if (reader.Name == "emailtitle")
                    {

                        reader.Read();
                        allMails.ElementAt(allMails.Count - 1).title = reader.Value;

                    }
                    else if (reader.Name == "messagebody")
                    {
                        reader.Read();
                       allMails.ElementAt(allMails.Count - 1).messageBody = SetXaml(reader.Value);

                    }
                    else if (reader.Name == "filepath")
                    {
                        reader.Read();
                        allMails.ElementAt(allMails.Count - 1).attachments.Add(reader.Value);

                    }                   
                }
            }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Source file couldn't open!");
            }
        }
        
        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "xml";
            openFileDialog.Filter = "XML Files (*.xml)|*.xml";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                takeFromXml(filePath);
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.Filter = "XML Files|*.xml";
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                writeToXml(filePath);
            }
        }

        private void folderClicked() {
            treeInbox.MouseLeftButtonUp += treeItem_MouseLeftButtonUp;
            treeSent.MouseLeftButtonUp += treeItem_MouseLeftButtonUp;
            treeTrash.MouseLeftButtonUp += treeItem_MouseLeftButtonUp;
            treeDrafts.MouseLeftButtonUp += treeItem_MouseLeftButtonUp;
        }

        public class MyItem
        {
            public int id { get; set; }
            public string Sender { get; set; }
            public string Title { get; set; }
        }

        private void treeItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            mailList.Items.Clear();
            if (sender == treeInbox)
            {
                mailListGridC1.Header = "Sender";
                for (int i = 0; i < allMails.Count; i++) {
                    if(allMails.ElementAt(i).type == "inbox")
                    mailList.Items.Add(new MyItem { Sender = allMails[i].senderAddress, Title = allMails[i].title, id = allMails[i].id });
                }
            }
            else if (sender == treeSent)
            {
                mailListGridC1.Header = "Recepient";
                for (int i = 0; i < allMails.Count; i++)
                {
                    if (allMails.ElementAt(i).type == "sent")
                        mailList.Items.Add(new MyItem { Sender = allMails[i].recipients[0].address, Title = allMails[i].title, id = allMails[i].id });
                }
            }
            else if (sender == treeTrash)
            {
                mailListGridC1.Header = "Sender/Receiver";
                for (int i = 0; i < allMails.Count; i++)
                {
                    if (allMails.ElementAt(i).type == "trash")
                        mailList.Items.Add(new MyItem { Sender = allMails[i].recipients[0].address, Title = allMails[i].title, id = allMails[i].id });
                }
            }
            else if (sender == treeDrafts)
            {
                mailListGridC1.Header = "Recepient";
                for (int i = 0; i < allMails.Count; i++)
                {
                    if (allMails.ElementAt(i).type == "draft")
                        mailList.Items.Add(new MyItem { Sender = allMails[i].recipients[0].address, Title = allMails[i].title, id = allMails[i].id });
                }
            }
        }

        private void listView_Click(object sender, RoutedEventArgs e)
        {
            MyItem item = (MyItem)(sender as ListView).SelectedItem;
            if (item != null)
            {
                for (int i = 0; i < allMails.Count; i++) {
                    if (item.id == allMails[i].id) {
                        txtMail.Text = "";
                        txtMail.Text += "From: "+ allMails[i].senderAddress;
                        txtMail.Text += "\nRecepients:";
                        for (int j = 0; j < allMails[i].recipients.Count; j++) {
                            if (allMails[i].recipients[j].role == "to") {
                                txtMail.Text += "\nTo: " + allMails[i].recipients[j].address;
                            }
                            else if (allMails[i].recipients[j].role == "cc") {
                                txtMail.Text += "\nCc: " + allMails[i].recipients[j].address;
                            }
                            else if (allMails[i].recipients[j].role == "bcc")
                            {
                                txtMail.Text += "\nBcc: " + allMails[i].recipients[j].address;
                            }
                        }
                        txtMail.Text += "\n" + allMails[i].title + "\n" + allMails[i].messageBody.Text;
                        if (allMails[i].attachments.Count > 0){
                            txtMail.Text += "\nAttachments:\n";
                            for (int j = 0; j < allMails[i].attachments.Count; j++) {
                                txtMail.Text += allMails[i].attachments[j] + "\n";
                            }
                        }
                    }
                }                
            }
        }

        private void btnOutlookView_Click(object sender, RoutedEventArgs e) { 
            DockPanel.SetDock(mailListPanel,Dock.Left);
            mailList.MaxHeight = 340;
        }

        private void btnThunderbirdView_Click(object sender, RoutedEventArgs e) {
            DockPanel.SetDock(mailListPanel, Dock.Top);
            mailList.MaxHeight = 120;
        }

        private void btnDelete_Click ( object sender, RoutedEventArgs e) {
            MyItem item = (MyItem)mailList.SelectedItem; 
            if (item != null) {
                for (int i = 0; i < allMails.Count; i++)
                {
                    if (item.id == allMails[i].id) {
                        if (allMails.ElementAt(i).type == "trash")
                        {
                            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to delete this message?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
                            if (messageBoxResult == MessageBoxResult.Yes)
                            {
                                allMails.RemoveAt(i);
                                txtMail.Text = "";
                            }
                            //update mailList
                            mailList.Items.Clear();
                            for (int j = 0; j < allMails.Count; j++)
                            {
                                if (allMails.ElementAt(j).type == "trash")
                                    mailList.Items.Add(new MyItem { Sender = allMails[j].senderAddress, Title = allMails[j].title, id = allMails[j].id });
                            }
                        }
                        else if(allMails.ElementAt(i).type == "inbox") {
                            allMails.ElementAt(i).type = "trash";
                            txtMail.Text = "";
                            //update mailList
                            mailList.Items.Clear();
                            for (int j = 0; j < allMails.Count; j++)
                            {
                                if (allMails.ElementAt(j).type == "inbox")
                                    mailList.Items.Add(new MyItem { Sender = allMails[j].senderAddress, Title = allMails[j].title, id = allMails[j].id });
                            }
                        }
                        else if (allMails.ElementAt(i).type == "sent")
                        {
                            allMails.ElementAt(i).type = "trash";
                            txtMail.Text = "";
                            mailList.Items.Clear();
                            for (int j = 0; j < allMails.Count; j++)
                            {
                                if (allMails.ElementAt(j).type == "sent")
                                    mailList.Items.Add(new MyItem { Sender = allMails[j].recipients[0].address, Title = allMails[j].title, id = allMails[j].id });
                            }
                        }
                        else if (allMails.ElementAt(i).type == "draft")
                        {
                            allMails.ElementAt(i).type = "trash";
                            txtMail.Text = "";
                            mailList.Items.Clear();
                            for (int j = 0; j < allMails.Count; j++)
                            {
                                if (allMails.ElementAt(j).type == "draft")
                                    mailList.Items.Add(new MyItem { Sender = allMails[j].recipients[0].address, Title = allMails[j].title, id = allMails[j].id });
                            }
                        }
                    }
                }
            }
        }

        private void btnNewEmail_Click(object sender, RoutedEventArgs e)
        {
            sendMail = new SendMail();
            myController = new mailSystemUC();
            DockPanel.SetDock(myController, Dock.Top);
            sendMail.bottomDockPanel.Children.Add(myController);
            sendMail.Show();
        }
        
        private void listView_DoubleClick(object sender, MouseButtonEventArgs e) {
            MyItem item = (MyItem)(sender as ListView).SelectedItem;
            SendMail viewer = new SendMail();
            viewer.Show();
            mailSystemUC myController = new mailSystemUC();
            DockPanel.SetDock(myController, Dock.Top);
            viewer.bottomDockPanel.Children.Add(myController);
            if (item != null) {
                for (int i = 0; i < allMails.Count; i++)
                {
                    if (item.id == allMails[i].id)
                    {
                        viewer.viewEmail(allMails[i], myController);
                    }
                }
            }
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

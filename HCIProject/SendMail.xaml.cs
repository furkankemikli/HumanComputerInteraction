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
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace HCIProject
{
    /// <summary>
    /// Interaction logic for SendMail.xaml
    /// </summary>
    public partial class SendMail : Window
    {
        private List<string> attachmentPaths = new List<string>();

        public SendMail()
        {
            InitializeComponent();
        }
        
        public void viewEmail(EmailItem myEmail, mailSystemUC myController)
        {
            recepientAddress.IsReadOnly = true;
            ccAddress.IsReadOnly = true;
            bccAddress.IsReadOnly = true;
            emailTitle.IsReadOnly = true;
            myController.emailContent.IsReadOnly = true;
            myController.viewEmail(myEmail);
            myController.textEditor.Visibility = Visibility.Hidden;
            sendOptions.Visibility = Visibility.Hidden;
            btnAttach.Visibility = Visibility.Hidden;
            btnCancelEmail.Visibility = Visibility.Hidden;
            btnSendEmail.Visibility = Visibility.Hidden;
            for (int i = 0; i < myEmail.recipients.Count; i++)
            {
                if (myEmail.recipients[i].role == "to")
                {
                    recepientAddress.Text += myEmail.recipients.ElementAt(i).address + ", ";
                }
                else if (myEmail.recipients[i].role == "cc")
                {
                    ccAddress.Text += myEmail.recipients[i].address + ", ";
                }
                else if (myEmail.recipients[i].role == "bcc")
                {
                    bccAddress.Text += myEmail.recipients[i].address + ", ";
                }
            }
            emailTitle.Text = myEmail.title;
            for(int i=0; i < myEmail.attachments.Count; i++){
                attachmentList.Items.Add(new MyItem { icon = myEmail.attachments[i], path = myEmail.attachments[i] });
            }
        }

        private void btnSendEmail_Click(object sender, RoutedEventArgs e)
        {
            if (recepientAddress.Text != "" && emailTitle.Text != "")
            {
                EmailItem newEmail = new EmailItem();
                Recepient newRecepient = new Recepient();
                newEmail.recipients = new List<Recepient>();
                newEmail.attachments = new List<string>();
                newEmail.senderAddress = "furkankemikli@student.um.si";
                newEmail.title = emailTitle.Text;
                newEmail.messageBody = new TextRange(  MainWindow.myController.emailContent.Document.ContentStart, MainWindow.myController.emailContent.Document.ContentEnd);
                newRecepient.address = recepientAddress.Text;
                newRecepient.role = "to";
                newEmail.recipients.Add(newRecepient);
                if (ccAddress.Text != "")
                {
                    newRecepient = new Recepient();
                    newRecepient.address = ccAddress.Text;
                    newRecepient.role = "cc";
                    newEmail.recipients.Add(newRecepient);
                }
                if (bccAddress.Text != "")
                {
                    newRecepient = new Recepient();
                    newRecepient.address = bccAddress.Text;
                    newRecepient.role = "bcc";
                    newEmail.recipients.Add(newRecepient);
                }
                newEmail.id = MainWindow.generalId;
                for(int i=0; i < attachmentPaths.Count; i++){
                    newEmail.attachments.Add(attachmentPaths[i]);
                }
                MainWindow.generalId = MainWindow.generalId + 1;
                newEmail.type = "sent";
                MainWindow.allMails.Add(newEmail);
                MainWindow.sendMail.Close();
            }
            else {
                MessageBox.Show("You have enter both recepient address and title of email!!!");
            }
        }

        public class MyItem
        {
            public string path { get; set; }
            public string icon { get; set; }
        }

        private void btnAttach_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "jpg";
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "File Types (*.jpg;*.png;*.gif;*.bmp;*.*)|*.jpg;*.png;*.gif;*.bmp;*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (String fileName in openFileDialog.FileNames) {
                    attachmentPaths.Add(fileName);
                    attachmentList.Items.Add(new MyItem { icon = fileName, path = fileName });
                }
            }
        }
    }
}

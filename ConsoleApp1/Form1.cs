namespace Asjc.ShellContextMenu.Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ShellContextMenu ctxMnu = new();
            FileInfo[] arrFI = [new FileInfo(@"C:\\Users\\23162\\Downloads\\ShellContextMenu-master\\ShellContextMenu-master")];
            ctxMnu.ShowContextMenu(arrFI, MousePosition, this.Handle);
        }
    }
}

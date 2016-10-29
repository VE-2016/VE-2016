namespace AIMS.Libraries.Scripting.ScriptControl
{
    partial class ErrorList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorList));
            this.lvErrorList = new System.Windows.Forms.ListView();
            this.colIcon = new System.Windows.Forms.ColumnHeader();
            this.colNo = new System.Windows.Forms.ColumnHeader();
            this.colDescription = new System.Windows.Forms.ColumnHeader();
            this.colFile = new System.Windows.Forms.ColumnHeader();
            this.colLine = new System.Windows.Forms.ColumnHeader();
            this.colColumn = new System.Windows.Forms.ColumnHeader();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // lvErrorList
            // 
            this.lvErrorList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvErrorList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colIcon,
            this.colNo,
            this.colDescription,
            this.colFile,
            this.colLine,
            this.colColumn});
            this.lvErrorList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvErrorList.FullRowSelect = true;
            this.lvErrorList.GridLines = true;
            this.lvErrorList.LargeImageList = this.imgList;
            this.lvErrorList.Location = new System.Drawing.Point(0, 0);
            this.lvErrorList.MultiSelect = false;
            this.lvErrorList.Name = "lvErrorList";
            this.lvErrorList.ShowGroups = false;
            this.lvErrorList.Size = new System.Drawing.Size(458, 157);
            this.lvErrorList.SmallImageList = this.imgList;
            this.lvErrorList.StateImageList = this.imgList;
            this.lvErrorList.TabIndex = 0;
            this.lvErrorList.UseCompatibleStateImageBehavior = false;
            this.lvErrorList.View = System.Windows.Forms.View.Details;
            this.lvErrorList.DoubleClick += new System.EventHandler(this.lvErrorList_DoubleClick);
            this.lvErrorList.Resize += new System.EventHandler(this.lvErrorList_Resize);
            // 
            // colIcon
            // 
            this.colIcon.Text = "";
            this.colIcon.Width = 16;
            // 
            // colNo
            // 
            this.colNo.Text = "";
            this.colNo.Width = 18;
            // 
            // colDescription
            // 
            this.colDescription.Text = "Description";
            this.colDescription.Width = 166;
            // 
            // colFile
            // 
            this.colFile.Text = "File";
            this.colFile.Width = 96;
            // 
            // colLine
            // 
            this.colLine.Text = "Line";
            this.colLine.Width = 36;
            // 
            // colColumn
            // 
            this.colColumn.Text = "Column";
            this.colColumn.Width = 54;
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Fuchsia;
            this.imgList.Images.SetKeyName(0, "ErrorList.bmp");
            this.imgList.Images.SetKeyName(1, "Output.bmp");
            this.imgList.Images.SetKeyName(2, "Warning.bmp");
            this.imgList.Images.SetKeyName(3, "Error.bmp");
            // 
            // ErrorList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 157);
            this.Controls.Add(this.lvErrorList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ErrorList";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TabText = "Error List";
            this.Text = "Error List";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvErrorList;
        private System.Windows.Forms.ColumnHeader colIcon;
        private System.Windows.Forms.ColumnHeader colNo;
        private System.Windows.Forms.ColumnHeader colDescription;
        private System.Windows.Forms.ColumnHeader colFile;
        private System.Windows.Forms.ColumnHeader colLine;
        private System.Windows.Forms.ColumnHeader colColumn;
        private System.Windows.Forms.ImageList imgList;
    }
}
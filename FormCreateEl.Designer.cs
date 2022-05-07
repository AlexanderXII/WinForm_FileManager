namespace KR_WinForm_FileManager
{
    partial class FormCreateEl
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
            this.NameElementTextBox = new System.Windows.Forms.TextBox();
            this.EnterBut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // NameElementTextBox
            // 
            this.NameElementTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NameElementTextBox.Location = new System.Drawing.Point(12, 21);
            this.NameElementTextBox.Multiline = true;
            this.NameElementTextBox.Name = "NameElementTextBox";
            this.NameElementTextBox.Size = new System.Drawing.Size(203, 20);
            this.NameElementTextBox.TabIndex = 0;
            this.NameElementTextBox.Text = "Имя папки";
            this.NameElementTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // EnterBut
            // 
            this.EnterBut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EnterBut.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.EnterBut.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.EnterBut.Location = new System.Drawing.Point(85, 57);
            this.EnterBut.Name = "EnterBut";
            this.EnterBut.Size = new System.Drawing.Size(70, 23);
            this.EnterBut.TabIndex = 1;
            this.EnterBut.Text = "Принять";
            this.EnterBut.UseVisualStyleBackColor = true;
            // 
            // FormCreateEl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 91);
            this.Controls.Add(this.EnterBut);
            this.Controls.Add(this.NameElementTextBox);
            this.Name = "FormCreateEl";
            this.Text = "Имя Элемента";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox NameElementTextBox;
        private System.Windows.Forms.Button EnterBut;
    }
}
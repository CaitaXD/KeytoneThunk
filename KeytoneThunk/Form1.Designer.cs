﻿namespace KeytoneThunk;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
        button1 = new System.Windows.Forms.Button();
        rtxtboxUserInput = new System.Windows.Forms.RichTextBox();
        SuspendLayout();
        // 
        // button1
        // 
        button1.Location = new System.Drawing.Point(484, 337);
        button1.Name = "button1";
        button1.Size = new System.Drawing.Size(213, 86);
        button1.TabIndex = 0;
        button1.Text = "Pray Musicus";
        button1.UseVisualStyleBackColor = true;
        button1.Click += button1_Click;
        // 
        // rtxtboxUserInput
        // 
        rtxtboxUserInput.Location = new System.Drawing.Point(155, 53);
        rtxtboxUserInput.Name = "rtxtboxUserInput";
        rtxtboxUserInput.Size = new System.Drawing.Size(542, 260);
        rtxtboxUserInput.TabIndex = 1;
        rtxtboxUserInput.Text = "";
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(800, 450);
        Controls.Add(rtxtboxUserInput);
        Controls.Add(button1);
        Text = "Form1";
        ResumeLayout(false);
    }

    private System.Windows.Forms.RichTextBox rtxtboxUserInput;

    private System.Windows.Forms.Button button1;

    #endregion
}
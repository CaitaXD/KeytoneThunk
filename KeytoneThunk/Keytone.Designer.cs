namespace KeytoneThunk;

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
        btnPlay = new System.Windows.Forms.Button();
        rtxtboxUserInput = new System.Windows.Forms.RichTextBox();
        btnSaveToMidiFile = new System.Windows.Forms.Button();
        lblVolume = new System.Windows.Forms.Label();
        txtboxVolume = new System.Windows.Forms.TextBox();
        txtboxBpm = new System.Windows.Forms.TextBox();
        lblBpm = new System.Windows.Forms.Label();
        lblSeed = new System.Windows.Forms.Label();
        btnRandSeed = new System.Windows.Forms.Button();
        txtboxSeed = new System.Windows.Forms.MaskedTextBox();
        btnLoadText = new System.Windows.Forms.Button();
        btnStop = new System.Windows.Forms.Button();
        SuspendLayout();
        // 
        // btnPlay
        // 
        btnPlay.Location = new System.Drawing.Point(219, 332);
        btnPlay.Name = "btnPlay";
        btnPlay.Size = new System.Drawing.Size(250, 86);
        btnPlay.TabIndex = 0;
        btnPlay.Text = "Play Music";
        btnPlay.UseVisualStyleBackColor = true;
        btnPlay.Click += button1_Click;
        // 
        // rtxtboxUserInput
        // 
        rtxtboxUserInput.Location = new System.Drawing.Point(219, 9);
        rtxtboxUserInput.Name = "rtxtboxUserInput";
        rtxtboxUserInput.Size = new System.Drawing.Size(542, 260);
        rtxtboxUserInput.TabIndex = 1;
        rtxtboxUserInput.Text = "";
        // 
        // btnSaveToMidiFile
        // 
        btnSaveToMidiFile.Location = new System.Drawing.Point(36, 350);
        btnSaveToMidiFile.Name = "btnSaveToMidiFile";
        btnSaveToMidiFile.Size = new System.Drawing.Size(109, 49);
        btnSaveToMidiFile.TabIndex = 2;
        btnSaveToMidiFile.Text = "Save To File";
        btnSaveToMidiFile.UseVisualStyleBackColor = true;
        btnSaveToMidiFile.Click += btnSaveToMidiFile_Click;
        // 
        // lblVolume
        // 
        lblVolume.Location = new System.Drawing.Point(10, 9);
        lblVolume.Name = "lblVolume";
        lblVolume.Size = new System.Drawing.Size(65, 24);
        lblVolume.TabIndex = 3;
        lblVolume.Text = "Volume";
        // 
        // txtboxVolume
        // 
        txtboxVolume.Location = new System.Drawing.Point(81, 9);
        txtboxVolume.Name = "txtboxVolume";
        txtboxVolume.ReadOnly = true;
        txtboxVolume.Size = new System.Drawing.Size(122, 23);
        txtboxVolume.TabIndex = 4;
        // 
        // txtboxBpm
        // 
        txtboxBpm.Location = new System.Drawing.Point(81, 43);
        txtboxBpm.Name = "txtboxBpm";
        txtboxBpm.ReadOnly = true;
        txtboxBpm.Size = new System.Drawing.Size(122, 23);
        txtboxBpm.TabIndex = 6;
        // 
        // lblBpm
        // 
        lblBpm.Location = new System.Drawing.Point(10, 43);
        lblBpm.Name = "lblBpm";
        lblBpm.Size = new System.Drawing.Size(65, 24);
        lblBpm.TabIndex = 5;
        lblBpm.Text = "BPM";
        // 
        // lblSeed
        // 
        lblSeed.Location = new System.Drawing.Point(10, 285);
        lblSeed.Name = "lblSeed";
        lblSeed.Size = new System.Drawing.Size(65, 24);
        lblSeed.TabIndex = 7;
        lblSeed.Text = "Seed";
        // 
        // btnRandSeed
        // 
        btnRandSeed.Location = new System.Drawing.Point(186, 285);
        btnRandSeed.Name = "btnRandSeed";
        btnRandSeed.Size = new System.Drawing.Size(69, 23);
        btnRandSeed.TabIndex = 9;
        btnRandSeed.Text = "Rand";
        btnRandSeed.UseVisualStyleBackColor = true;
        btnRandSeed.Click += btnRandSeed_Click;
        // 
        // txtboxSeed
        // 
        txtboxSeed.Location = new System.Drawing.Point(47, 285);
        txtboxSeed.Mask = "000000000000000000";
        txtboxSeed.Name = "txtboxSeed";
        txtboxSeed.Size = new System.Drawing.Size(133, 23);
        txtboxSeed.TabIndex = 10;
        // 
        // btnLoadText
        // 
        btnLoadText.Location = new System.Drawing.Point(686, 275);
        btnLoadText.Name = "btnLoadText";
        btnLoadText.Size = new System.Drawing.Size(75, 23);
        btnLoadText.TabIndex = 11;
        btnLoadText.Text = "Load Text";
        btnLoadText.UseVisualStyleBackColor = true;
        btnLoadText.Click += btnLoadText_Click;
        // 
        // btnStop
        // 
        btnStop.Location = new System.Drawing.Point(500, 332);
        btnStop.Name = "btnStop";
        btnStop.Size = new System.Drawing.Size(250, 86);
        btnStop.TabIndex = 12;
        btnStop.Text = "Stop\r\n";
        btnStop.UseVisualStyleBackColor = true;
        btnStop.Click += btnStop_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(800, 450);
        Controls.Add(btnStop);
        Controls.Add(btnLoadText);
        Controls.Add(txtboxSeed);
        Controls.Add(btnRandSeed);
        Controls.Add(lblSeed);
        Controls.Add(txtboxBpm);
        Controls.Add(lblBpm);
        Controls.Add(txtboxVolume);
        Controls.Add(lblVolume);
        Controls.Add(btnSaveToMidiFile);
        Controls.Add(rtxtboxUserInput);
        Controls.Add(btnPlay);
        Text = "Form1";
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.Button btnStop;

    private System.Windows.Forms.Button btnLoadText;

    private System.Windows.Forms.MaskedTextBox txtboxSeed;

    private System.Windows.Forms.Button btnRandSeed;

    private System.Windows.Forms.Label lblSeed;

    private System.Windows.Forms.TextBox txtboxBpm;
    private System.Windows.Forms.Label lblBpm;

    private System.Windows.Forms.Label lblVolume;
    private System.Windows.Forms.TextBox txtboxVolume;

    private System.Windows.Forms.Button btnSaveToMidiFile;

    private System.Windows.Forms.RichTextBox rtxtboxUserInput;

    private System.Windows.Forms.Button btnPlay;

    #endregion
}
﻿namespace GenParametroWU
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbGrupos = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnEjecutar = new System.Windows.Forms.Button();
            this.pgbrEjecucion = new System.Windows.Forms.ProgressBar();
            this.tablaXml = new System.Windows.Forms.Button();
            this.comboTablas = new System.Windows.Forms.ComboBox();
            this.btnFromXML = new System.Windows.Forms.Button();
            this.btnPaises = new System.Windows.Forms.Button();
            this.cbCountries = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cbGrupos
            // 
            this.cbGrupos.FormattingEnabled = true;
            this.cbGrupos.Location = new System.Drawing.Point(101, 60);
            this.cbGrupos.Margin = new System.Windows.Forms.Padding(4);
            this.cbGrupos.Name = "cbGrupos";
            this.cbGrupos.Size = new System.Drawing.Size(195, 24);
            this.cbGrupos.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 64);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Grupos";
            // 
            // btnEjecutar
            // 
            this.btnEjecutar.Location = new System.Drawing.Point(384, 57);
            this.btnEjecutar.Name = "btnEjecutar";
            this.btnEjecutar.Size = new System.Drawing.Size(93, 31);
            this.btnEjecutar.TabIndex = 2;
            this.btnEjecutar.Text = "Ejecutar";
            this.btnEjecutar.UseVisualStyleBackColor = true;
            this.btnEjecutar.Click += new System.EventHandler(this.btnEjecutar_Click);
            // 
            // pgbrEjecucion
            // 
            this.pgbrEjecucion.Location = new System.Drawing.Point(101, 131);
            this.pgbrEjecucion.Name = "pgbrEjecucion";
            this.pgbrEjecucion.Size = new System.Drawing.Size(376, 29);
            this.pgbrEjecucion.TabIndex = 3;
            // 
            // tablaXml
            // 
            this.tablaXml.Location = new System.Drawing.Point(673, 60);
            this.tablaXml.Name = "tablaXml";
            this.tablaXml.Size = new System.Drawing.Size(199, 41);
            this.tablaXml.TabIndex = 4;
            this.tablaXml.Text = "Guardar Tabla a XML";
            this.tablaXml.UseVisualStyleBackColor = true;
            this.tablaXml.Click += new System.EventHandler(this.tablaXml_Click);
            // 
            // comboTablas
            // 
            this.comboTablas.FormattingEnabled = true;
            this.comboTablas.Location = new System.Drawing.Point(673, 131);
            this.comboTablas.Name = "comboTablas";
            this.comboTablas.Size = new System.Drawing.Size(201, 24);
            this.comboTablas.TabIndex = 5;
            // 
            // btnFromXML
            // 
            this.btnFromXML.Location = new System.Drawing.Point(164, 295);
            this.btnFromXML.Name = "btnFromXML";
            this.btnFromXML.Size = new System.Drawing.Size(132, 56);
            this.btnFromXML.TabIndex = 6;
            this.btnFromXML.Text = "Cargar datos desde XML";
            this.btnFromXML.UseVisualStyleBackColor = true;
            this.btnFromXML.Click += new System.EventHandler(this.btnFromXML_Click);
            // 
            // btnPaises
            // 
            this.btnPaises.Location = new System.Drawing.Point(724, 358);
            this.btnPaises.Name = "btnPaises";
            this.btnPaises.Size = new System.Drawing.Size(100, 42);
            this.btnPaises.TabIndex = 7;
            this.btnPaises.Text = "Procesar";
            this.btnPaises.UseVisualStyleBackColor = true;
            this.btnPaises.Click += new System.EventHandler(this.btnPaises_Click);
            // 
            // cbCountries
            // 
            this.cbCountries.FormattingEnabled = true;
            this.cbCountries.Location = new System.Drawing.Point(654, 312);
            this.cbCountries.Name = "cbCountries";
            this.cbCountries.Size = new System.Drawing.Size(236, 24);
            this.cbCountries.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.cbCountries);
            this.Controls.Add(this.btnPaises);
            this.Controls.Add(this.btnFromXML);
            this.Controls.Add(this.comboTablas);
            this.Controls.Add(this.tablaXml);
            this.Controls.Add(this.pgbrEjecucion);
            this.Controls.Add(this.btnEjecutar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbGrupos);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbGrupos;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnEjecutar;
        private System.Windows.Forms.ProgressBar pgbrEjecucion;
        private System.Windows.Forms.Button tablaXml;
        private System.Windows.Forms.ComboBox comboTablas;
        private System.Windows.Forms.Button btnFromXML;
        private System.Windows.Forms.Button btnPaises;
        private System.Windows.Forms.ComboBox cbCountries;
    }
}


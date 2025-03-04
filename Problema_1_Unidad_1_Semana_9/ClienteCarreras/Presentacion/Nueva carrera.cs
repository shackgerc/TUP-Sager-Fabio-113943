﻿using Aplicacion.Dominio;
using Aplicacion.Servicios.Implementaciones;
using Aplicacion.Servicios.Interfaces;
using ClienteCarreras.Cliente;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ClienteCarreras.Presentacion
{
    public partial class frmNuevaCarrera : Form
    {
        Carrera carrera;
        IServicio servicio;

        public frmNuevaCarrera()
        {
            InitializeComponent();
            carrera = new Carrera();
            servicio = new ServicioCarrera();
        }

        private async Task CargarCboMateriasAsync()
        {
            string url = "http://localhost:5225/api/ControllerAsignaturas";
            var data = await ClienteSingleton.ObtenerInstancia().GetAsync(url);
            List<Asignatura> asignaturas = JsonConvert.DeserializeObject<List<Asignatura>>(data);
            cboMaterias.DataSource = asignaturas;
            cboMaterias.ValueMember = "Codigo";
            cboMaterias.DisplayMember = "Nombre";
            cboMaterias.SelectedIndex = -1;
        }

        private async void frmNuevaCarrera_Load(object sender, EventArgs e)
        {
            await CargarCboMateriasAsync();
        }

        private void btnAgregarDetalle_Click(object sender, EventArgs e)
        {
            if (txtNombreCarrera.Text.Equals(String.Empty))
            {
                MessageBox.Show("Debe ingresar el nombre de la carrera!",
                "Control", MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
                return;
            }

            if (cboMaterias.Text.Equals(String.Empty))
            {
                MessageBox.Show("Debe seleccionar una Materia!",
                "Control", MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
                return;
            }

            if (txtAnioCursado.Text == "")
            {
                MessageBox.Show("Debe ingresar un año de cursado valido!",
                "Control", MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
                return;
            }

            if (rbnPrimerCuatrimestre.Checked == false && rbnSegundoCuatrimestre.Checked == false)
            {
                MessageBox.Show("Debe seleccionar un cuatrimestre!",
                "Control", MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
                return;
            }

            foreach (DetalleCarrera dc in carrera.DetallesCarrera)
            {
                if (dc.Materia.Nombre == cboMaterias.Text)
                {
                    MessageBox.Show("Esa materia ya existe en esta carrera!",
                    "Control", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                    return;
                }
            }

            Asignatura materia = new Asignatura();
            materia.Codigo = Convert.ToInt32(cboMaterias.SelectedValue);
            materia.Nombre = cboMaterias.Text;

            int anioCursado = int.Parse(txtAnioCursado.Text);
            int cuatrimestre = 2;
            if (rbnPrimerCuatrimestre.Checked)
            {
                cuatrimestre = 1;
            }

            DetalleCarrera detalle = new DetalleCarrera(anioCursado, cuatrimestre, materia);

            carrera.AgregarDetalle(detalle);

            dgvDetalles.Rows.Add(materia.Codigo, materia.Nombre, anioCursado, cuatrimestre);
        }

        private void dgvDetalles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDetalles.CurrentCell.ColumnIndex == 4)
            {
                carrera.EliminarDetalle(dgvDetalles.CurrentCell.RowIndex);
                dgvDetalles.Rows.Remove(dgvDetalles.CurrentRow);
            }
        }

        private void txtNombreCarrera_TextChanged(object sender, EventArgs e)
        {
            carrera.NombreTitulo = txtNombreCarrera.Text;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private async void btnAceptar_Click(object sender, EventArgs e)
        {
            if (txtNombreCarrera.Text == "")
            {
                MessageBox.Show("Debe ingresar el nombre de la carrera!",
                "Control", MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
                return;
            }

            string url = "http://localhost:5225/carreraPost";
            string carreraJson = JsonConvert.SerializeObject(carrera);

            var result = await ClienteSingleton.ObtenerInstancia().PostAsync(url, carreraJson);

            if (!result.Equals("true"))
            {
                MessageBox.Show("Error, no se pudo cargar la carrera", "Insertar",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("La carrera se inserto con exito", "Insertar",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

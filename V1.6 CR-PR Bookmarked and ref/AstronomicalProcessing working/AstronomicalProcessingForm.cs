﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Original Code: Machyl 30017609, C Blunt, Sprint One
// Modified Code: Bruce Fisher P197681, C Blunt, Sprint One
// Date: 18/09/21
// Version: v1.6 -> lists CR: Client Requirements & PR: Program Requirements for CODE and FORM DESIGNER
// Astronomical Processing
// Program lists continuous data collected from the interaction of Neutrinos with Earth matter.
// The program will utilise simulated data and allows the user to add, edit, delete, sort and search the data.

// CR: 5	The name of the application should be Astronomical Processing.
namespace AstronomicalProcessing
{
    public partial class AstronomicalProcessingForm : Form
    {
        // CR: 1	All data is stored as integers in an array.
        // PR: 1	The array is of type integer.
        // PR: 2	The array has 24 elements to reflect the number of hours per day.
        int?[] NeutrinoInteractions = new int?[24];

        int SelectedIndex = -1; // Set nothing selected from list box
        int NumberOfDataEntries = 0;
        readonly String BLANK_ENTRY = "-";
        bool isSorted = false;     

        #region Initialise Form Components
        /// <summary>
        /// Initialises Form Components
        /// </summary>
        public AstronomicalProcessingForm()
        {
            InitializeComponent();

            // Tool Tips for mouse cursor hovering over Buttons and Text Box
            toolTip1.SetToolTip(TextBoxMain, "Input Value to process");
            toolTip1.SetToolTip(ButtonAdd, "Add Value To List - from given input");
            toolTip1.SetToolTip(ButtonEdit, "Edit Value In List - from selection in list");
            toolTip1.SetToolTip(ButtonDelete, "Delete Value In List - from selection in list");
            toolTip1.SetToolTip(ButtonSort, "Sort Values - ascending order");
            toolTip1.SetToolTip(ButtonSearch, "Search For First Value - from given input");
            toolTip1.SetToolTip(ButtonAutoFill, "Pre-Fill List - random values");
            toolStripLabel1.Text = ""; // initialise to no text in toolstrip label

            TextBoxMain.MaxLength = 2; // fix textbox to only accept input of 2 digits
        }
        #endregion
        #region Add Button
        /// <summary>
        /// Adds value to the array NeutrinoInteractions
        /// </summary>

        // PR: 9	The program must be able to add, edit and delete data values.
        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            String inputText = TextBoxMain.Text;
            int inputInt;

            if (Int32.TryParse(inputText, out inputInt)) // Check for invalid input
            {
                if (NumberOfDataEntries < NeutrinoInteractions.Length)
                {
                    for (int i = 0; i < NeutrinoInteractions.Length; i++) //for looking for empty slots
                    {
                        if (!NeutrinoInteractions[i].HasValue)
                        {
                            NeutrinoInteractions[i] = inputInt;
                            DisplayToLabelMsg("Added value : " + inputInt);
                            NumberOfDataEntries++;
                            SelectedIndex = -1; // Set nothing selected from list box
                            UpdateDisplay();
                            TextBoxMain.Clear();
                            isSorted = false;
                            return;
                        }
                    }
                    DisplayToLabelMsg("Application Error - Couldn't find null to fill in array");
                    return;
                }
                DisplayToLabelMsg("Error - List already full, max : " + NeutrinoInteractions.Length);
                return;
            }
            TextBoxMain.Clear(); // clear invalid entry
            // PR: 6	The program must generate an error message if the text box is empty.
            DisplayToLabelMsg("Error - Please enter an integer");
        }
        #endregion
        #region Edit Button
        /// <summary>
        /// Edits value in the array NeutrinoInteractions
        /// </summary>

        // PR: 9	The program must be able to add, edit and delete data values.
        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            if (SelectedIndex == -1) // Check data is selected from list box
            {
                // PR: 6	The program must generate an error message if the text box is empty.
                DisplayToLabelMsg("Error - Please select existing value from list");
                return;
            }
            String inputText = TextBoxMain.Text;
            int dataDeleted = (int)NeutrinoInteractions[SelectedIndex]; // store old value to print in message label
            int inputInt;

            if (Int32.TryParse(inputText, out inputInt)) // Check for invalid input
            {
                NeutrinoInteractions[SelectedIndex] = inputInt;
                DisplayToLabelMsg("Value " + dataDeleted + " Edited to : " + inputInt);
                SelectedIndex = -1; // Set nothing selected from list box
                UpdateDisplay();
                TextBoxMain.Clear();
                isSorted = false;
                return;
            }
            TextBoxMain.Clear(); // clear invalid entry
            // PR: 6	The program must generate an error message if the text box is empty.
            DisplayToLabelMsg("Error - Please enter an integer");
        }
        #endregion
        #region Delete Button
        /// <summary>
        /// Deletes value from the array NeutrinoInteractions from given textbox input
        /// </summary>

        // PR: 9	The program must be able to add, edit and delete data values.
        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            if (SelectedIndex == -1) // Check data is selected from list box
            {
                // PR: 6	The program must generate an error message if the text box is empty.
                DisplayToLabelMsg("Error - Please select existing value from list");
                return;
            }
            
            int dataDeleted = (int)NeutrinoInteractions[SelectedIndex]; // store old value used to print in message labels
           
            DialogResult DeleteValue = MessageBox.Show("Are you sure you want to Delete this value?", "Confirmation",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
           
            if (DeleteValue == DialogResult.Yes)
            {
                NeutrinoInteractions[SelectedIndex] = null;
                DisplayToLabelMsg("Deleted value : " + dataDeleted);
                SelectedIndex = -1; // Set nothing selected from list box
                NumberOfDataEntries--;
                UpdateDisplay();
                TextBoxMain.Clear();
                isSorted = false;
            }
            else
                DisplayToLabelMsg("Did NOT Delete Value : " + dataDeleted);
        }
        #endregion
        #region Sort Button
        /// <summary>
        /// Sorts data in the array NeutrinoInteractions in ascending order.
        /// And updates the display with sorted data.
        /// </summary>
        private void ButtonSort_Click(object sender, EventArgs e)
        {
            SelectedIndex = -1; // Set nothing selected from list box
            TextBoxMain.Clear(); // clear any old text box item
            SortIntArray();
            UpdateDisplay();
        }
        #endregion
        #region Search Button
        /// <summary>
        /// Searches for given value from textbox in the array NeutrinoInteractions
        /// </summary>
        private void ButtonSearch_Click(object sender, EventArgs e)
        {
            if (!isSorted)
            {
                DisplayToLabelMsg("Error - Please Sort list first");
                return;
            }

            String searchText = TextBoxMain.Text;
            if (searchText.Equals(""))
            {
                // PR: 6	The program must generate an error message if the text box is empty.
                DisplayToLabelMsg("Error - Text box empty, please enter value to Search");
                return;
            }

            int inputInt;
            if (!Int32.TryParse(searchText, out inputInt)) // Check for invalid input
            {
                TextBoxMain.Clear(); // clear invalid entry
                DisplayToLabelMsg("Error - Please enter an integer");
                return;
            }

            #region Binary Search
            // PR: 4	The search method must be coded using the Binary Search algorithm.
            int min = 0;
            int max = NeutrinoInteractions.Length - 1;
            int mid;

            while (min <= max)
            {
                mid = (min + max) / 2;
                if (NeutrinoInteractions[mid].HasValue && NeutrinoInteractions[mid].Value == inputInt)
                {
                    SelectedIndex = mid; // set item found in list
                    ListBoxMain.SetSelected(mid, true);
                    // PR: 8	The program must generate a message if the search is successful.
                    DisplayToLabelMsg("Value Found : " + NeutrinoInteractions[mid] + " at index " + (mid + 1));
                    SelectedIndex = -1; // Set nothing selected from list box
                    TextBoxMain.Clear(); // clear search item
                    return;
                }
                else
                {
                    if (NeutrinoInteractions[mid].HasValue && NeutrinoInteractions[mid].Value > inputInt)
                    {
                        max = mid - 1;
                    }
                    else if (!NeutrinoInteractions[mid].HasValue || NeutrinoInteractions[mid].Value < inputInt) //assuming nulls where sorted to start of array
                    {
                        min = mid + 1;
                    }
                }
                // PR: 7	The program must generate an error message if the search is not successful.
                DisplayToLabelMsg("Value NOT Found : " + inputInt);
                SelectedIndex = -1; // Set nothing selected from list box
                TextBoxMain.Clear(); // clear search item
            }
            #endregion
        }
        #endregion
        #region Auto Fill Button
        /// <summary>
        /// Fills values into the array NeutrinoInteractions and updates the display
        /// </summary>
        private void ButtonAutoFill_Click(object sender, EventArgs e)
        {
            AutofillData();
            UpdateDisplay();
        }
        #endregion
        #region Utilities
        #region AutoFillData
        /// <summary>
        /// Fills values into the array NeutrinoInteractions with random numbers in the range 10 to 99.
        /// For generating random test data.
        /// </summary>

        // PR: 10	The array is filled with random integers to simulate the data stream (numbers between 10 and 99).
        private void AutofillData()
        {
            Random rnd = new Random();
            for (int i = 0; i < NeutrinoInteractions.Length; i++) //for looking for empty slots
            {
                if (!NeutrinoInteractions[i].HasValue)
                {
                    NeutrinoInteractions[i] = rnd.Next(10, 100);
                    NumberOfDataEntries++;
                    continue;
                }
            }
            DisplayToLabelMsg("Data has been pre-filled");
            isSorted = false;
        }
        #endregion
        #region Sort Array
        /// <summary>
        /// Sorts data in the array NeutrinoInteractions in ascending order
        /// </summary>
        private void SortIntArray()
        {
            //setting nulls to -1 before sorting
            for (int i = 0; i < NeutrinoInteractions.Length; i++)
            {
                if (!NeutrinoInteractions[i].HasValue)
                {
                    NeutrinoInteractions[i] = -1;
                }
            }

            #region Bubble Sort
            // PR: 3	The sort method must be coded using the Bubble Sort algorithm.
            int int1, int2;
            for (int i = 0; i < NeutrinoInteractions.Length; i++)
            {
                for (int j = 0; j + 1 < NeutrinoInteractions.Length; j++)
                {
                    int1 = NeutrinoInteractions[j].Value;
                    int2 = NeutrinoInteractions[j + 1].Value;
                    if (int1 > int2)
                    {
                        NeutrinoInteractions[j] = int2;
                        NeutrinoInteractions[j + 1] = int1;
                    }
                }
            }
            DisplayToLabelMsg("Data has been Sorted in ascending order");
            #endregion

            //set -1 values back to null after sorting
            for (int i = 0; i < NeutrinoInteractions.Length; i++)
            {
                if (NeutrinoInteractions[i] == -1)
                {
                    NeutrinoInteractions[i] = null;
                }
            }

            isSorted = true;
        }
        #endregion
        #region UpdateDisplay List Box
        /// <summary>
        /// Fills in listbox with data from NeutrinoInteractions array
        /// </summary>
        private void UpdateDisplay()
        {
            ListBoxMain.Items.Clear();
            for (int i = 0; i < NeutrinoInteractions.Length; i++)
            {
                if (NeutrinoInteractions[i].HasValue)
                    ListBoxMain.Items.Add(NeutrinoInteractions[i]);
                else
                    ListBoxMain.Items.Add(BLANK_ENTRY);
            }
        }
        #endregion
        #region ListBoxMain Selected Index Changed
        /// <summary>
        /// Sets the SelectedIndex upon clicking a selection from listbox values
        /// </summary>
        private void ListBoxMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            string curItem = ListBoxMain.SelectedItem.ToString();
            if (curItem == BLANK_ENTRY)
            {
                SelectedIndex = -1; // Set nothing selected from list box
                TextBoxMain.Text = "";
            }
            else
            {
                SelectedIndex = ListBoxMain.SelectedIndex;
                TextBoxMain.Text = curItem;
            }
        }
        #endregion
        #region DisplayToolLabelMsg
        // <summary>
        /// Displays string parameter given onto toolstriplabel and flashes 
        /// label to draw attention to user that message has been updated
        /// </summary>
        private void DisplayToLabelMsg(String message)
        {
            toolStripLabel1.Text = message;
            FlashToolLabel();
        }
        #endregion
        #region FlashToolLabel
        /// <summary>
        /// Flashes tool tip by changing toolstriplabel visable on/off 
        /// to bring attention to error message
        /// </summary>
        private void FlashToolLabel()
        {
            toolStrip1.Visible = false;
            System.Threading.Thread.Sleep(100); // wait time between visability of tooltiplabel
            toolStrip1.Visible = true;
        }
        #endregion
        #endregion
    }
}

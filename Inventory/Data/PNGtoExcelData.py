import os
import pandas as pd

# Define paths
excel_file = 'C:/Users/morte/OneDrive - Danmarks Tekniske Universitet/ProjektGodot/Rapid RPG/ItemDataExcel.xlsx'
png_directory = 'C:/Users/morte/OneDrive - Danmarks Tekniske Universitet/ProjektGodot/Rapid RPG/Inventory/ItemIcons'

# Load the existing Excel file into a DataFrame
df = pd.read_excel(excel_file, engine='openpyxl')

# Get a list of all PNG files in the specified directory
png_files = [f for f in os.listdir(png_directory) if f.endswith('.png')]

# Extract item names from the PNG file names (assuming the file name is the item name)
png_item_names = [os.path.splitext(f)[0] for f in png_files]

# Check if each item name exists in the DataFrame
existing_item_names = df['ItemName'].tolist()

# Initialize a list to store new items
new_items = []

for item_name in png_item_names:
    if item_name not in existing_item_names:
        # Add a new row for the item
        new_item = {
            'ItemName': item_name,
            'ItemCategory': "Default",  # Add default or placeholder values as needed
            'StackSize': 1,      # Default stack size
            'Description': 'Default',   # Default description
            'Weight': 0          # Default weight
        }
        new_items.append(new_item)

# Append new items to the DataFrame using concat
if new_items:
    new_items_df = pd.DataFrame(new_items)
    df = pd.concat([df, new_items_df], ignore_index=True)

# Save the updated DataFrame back to the Excel file using openpyxl
with pd.ExcelWriter(excel_file, engine='openpyxl', mode='a', if_sheet_exists='replace') as writer:
    df.to_excel(writer, index=False, sheet_name='Sheet1')

print(f"Added {len(new_items)} new items to the Excel sheet.")
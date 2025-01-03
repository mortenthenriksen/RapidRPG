import os
import pandas as pd
import json

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
            'StackSize': 1,             # Default stack size
            'Description': 'Default',   # Default description
            'Weight': 0                 # Default weight
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


df = pd.read_excel(excel_file)

# Initialize an empty dictionary to store the JSON data
json_data = {}

# Iterate over each row in the DataFrame
for index, row in df.iterrows():
    item_name = row['ItemName']
    item_data = {
        "ItemCategory": row['ItemCategory'],
        "StackSize": row['StackSize'],
        "Description": row['Description'],
        "Weight": row['Weight']
    }
    
    # Add optional fields if they exist
    if 'Attack' in row and not pd.isna(row['Attack']):
        item_data['Attack'] = row['Attack']
    if 'AttackSpeed' in row and not pd.isna(row['AttackSpeed']):
        item_data['AttackSpeed'] = row['AttackSpeed']
    if 'Defense' in row and not pd.isna(row['Defense']):
        item_data['Defense'] = row['Defense']
    if 'MainStat' in row and not pd.isna(row['MainStat']):
        item_data['MainStat'] = row['MainStat']
    if 'UniqueEffect' in row and not pd.isna(row['UniqueEffect']):
        item_data['UniqueEffect'] = row['UniqueEffect']

    # Add the item data to the JSON dictionary
    json_data[item_name] = item_data

# Convert the dictionary to a JSON string
json_string = json.dumps(json_data, indent=4)

output_folder = "C:/Users/morte/OneDrive - Danmarks Tekniske Universitet/ProjektGodot/Rapid RPG/Inventory/Data"
output_file = os.path.join(output_folder, 'ItemDataFromExcel.json')


# Save the JSON string to a file
with open(output_file, 'w') as json_file:
    json_file.write(json_string)

# print(os.path.dirname(pd.__file__))

print("Excel file has been converted to JSON successfully.")
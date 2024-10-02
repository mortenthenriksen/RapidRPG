import os
import pandas as pd
import json


# run this whenever you add new items B-)
# Load the Excel file
excel_file = 'C:/Users/morte/OneDrive - Danmarks Tekniske Universitet/ProjektGodot/Rapid RPG/Inventory/Data/ItemDataExcel.xlsx'
df = pd.read_excel(excel_file)

# Initialize an empty dictionary to store the JSON data
json_data = {}

# Iterate over each row in the DataFrame
for index, row in df.iterrows():
    item_name = row['ItemName']
    item_data = {
        "ItemCategory": row['ItemCategory'],
        "StackSize": row['StackSize'],
        "Description": row['Description']
    }
    
    # Add optional fields if they exist
    if 'Attack' in row and not pd.isna(row['Attack']):
        item_data['Attack'] = row['Attack']
    if 'ItemSpeed' in row and not pd.isna(row['ItemSpeed']):
        item_data['ItemSpeed'] = row['ItemSpeed']
    if 'Defense' in row and not pd.isna(row['Defense']):
        item_data['Defense'] = row['Defense']
    if 'MainStat' in row and not pd.isna(row['MainStat']):
        item_data['MainStat'] = row['MainStat']
    if 'AddHealth' in row and not pd.isna(row['AddHealth']):
        item_data['AddHealth'] = row['AddHealth']
    if 'AddEnergy' in row and not pd.isna(row['AddEnergy']):
        item_data['AddEnergy'] = row['AddEnergy']
    
    # Add the item data to the JSON dictionary
    json_data[item_name] = item_data

# Convert the dictionary to a JSON string
json_string = json.dumps(json_data, indent=4)

output_folder = "C:/Users/morte/OneDrive - Danmarks Tekniske Universitet/ProjektGodot/Rapid RPG/Inventory/Data"
output_file = os.path.join(output_folder, 'ItemDataFromExcel.json')


# Save the JSON string to a file
with open(output_file, 'w') as json_file:
    json_file.write(json_string)

print(os.path.dirname(pd.__file__))

print("Excel file has been converted to JSON successfully.")
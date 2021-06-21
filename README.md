# Amicitia.github.io
This is a static set of HTML pages created dynamically using a C# program that also lives in this repository.
The program iterates through entries in a database to fill these pages with "posts" (mods/tools/cheats/guides with user-provided metadata).

# Why Static?
Github Pages provides a free and collaborative hosting solution, but only serves static pages.
While a self-hosted website could allow dynamic page building, the functionality can be replicated to work on Github using the included program.
Project Collaborators can submit their own changes to the page building code or database without using anything but GitHub and their editing tools of choice.
Now the community can work together to host material without relying on a single webmaster.

# How to Contribute to the Database?
## Google Sheets
- Clone the repository using Git or Github Desktop.
- Upload the .tsv file in the "db" folder to your Google Drive account.
- Open it in Google Sheets, add/edit rows as needed.
- Download as .tsv when finished and replace the file in the "db" folder.
## Microsoft Excel
- Clone the repository using Git or Github Desktop.
- Open the .xlsx file in the "db" folder.
- Add/edit rows as needed.
- Enable the Developer Tab, go to it and choose Visual Basic.
- Use the included ExportTsvExcel.cls script to export as .tsv.
- Replace the file in the "db" folder.

# How to Generate New Pages?
- Open the solution (.sln) in Visual Studio (or your preferred IDE).
- Run the program to generate new pages.
- Commit changes to your own fork and open a pull request.

# Credits
## Gamebanana Webscraper
Code from [TekkaGB's AemulusModManager](https://github.com/TekkaGB/AemulusModManager) was used to achieve webscraping using [GBAPIv4](https://gamebanana.com/apiv4/).  
As such, this project is licensed under GPL-3.0.
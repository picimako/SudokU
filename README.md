# SudokU

SudokU is basically a Sudoku game where you can solve Sudoku exercises. It is a refactored version of one of my college projects.

It can create and meanwhile solve different types of exercises:
- Simple sudoku
- Sudoku-X
- Center Dot
- Killer Sudoku combined with the above ones

It supports not just generating but importing exercises from simple text files in a given format, if you e.g. stuck with one in a newspaper and you need some hint or validation help.

The application also includes some basic options for validating the cells during and after solving the exercise.

# Future plans and todos

Plans include the followings:
- it currently has many non-English variable/method etc. names and comments, that should be changed to English
- remove unnecessary comments
- refactor and optimize code
- add logger to make debugging easier
- move configuration and language resource files out from Debug folder (they're currently refered to according to the path of the .exe file)
- move forms and localization to XAML based implementation
- add unit test project
- include functional documentation about the generator and solver algorithms to the project Wiki page
- add an "Export exercise" option with a DEV mode option to be able to export and later import problematic cases
- options to be able to track the numbers whiches all instances you have already filled in

## Minor technical details
The .exe file for the application currently can be found in the Sudoku/bin/Debug folder as the build creates it.
The language files are in the Sudoku/bin/Debug/Languages folder.
The settings can be found in Sudoku/bin/Debug/config.xml.

There are plans to move them to appropriate places.
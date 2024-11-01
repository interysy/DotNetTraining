Feature: Update Empty Book
 
@mytag12
Scenario: Updating a book 
    Given the user wants to update book 1 with no authors and provides "Updated Book", "Updated Book Description", 20.0, 1
	When the book is updated 
	Then the book properties should be updated in the database

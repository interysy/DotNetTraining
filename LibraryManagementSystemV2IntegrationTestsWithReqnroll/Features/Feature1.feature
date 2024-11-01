Feature: Feature1

A short summary of the feature

@tag1
Scenario: Updating book with no authors
	Given the user wants to update book 2 with no authors and provides "Book Updated", "Book Updated Description", 20.0, 1
	When the book is updated
	Then the book properties should be updated in the database

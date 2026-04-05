@account-opening
Feature: Account Opening

The process of opening an account, including various ways that an account opening can be unsuccessful

Rule: A user can open an account with valid details, but cannot open an account with missing or invalid information
	
	Scenario: Open an account with valid details
		Given I am an applicant for a new account with all valid details
		When I submit the application
		Then I should see confirmation my account has been opened

	Scenario: Open account with invalid SSN
		Given I am an applicant for a new account with an invalid SSN
		When I submit the application
		Then I should see an error message about the invalid SSN

	Scenario: Open account with exactly sufficient initial deposit
		Given I am an applicant for a new account with initial deposit of 200
		When I submit the application
		Then I should see confirmation my account has been opened

	Scenario: Open account with one more than sufficient initial deposit
		Given I am an applicant for a new account with initial deposit of 201
		When I submit the application
		Then I should see confirmation my account has been opened

	Scenario: Open account with one less than sufficient initial deposit
		Given I am an applicant for a new account with initial deposit of 199
		When I submit the application
		Then I should see an error message about insufficient initial deposit


Feature: Transactions

The transaction subsystem handles deposits and withdrawls from an account

@Deposits
Rule: Deposits must have a positive amount

	Scenario: Deposit with positive amount
		Given I want to deposit a positive amount into my account
		When I make the deposit
		Then The transfer should be approved

	Scenario: Deposit with zero amount
		Given I want to deposit a zero amount into my account
		When I make the deposit
		Then The transfer should be cancelled

@Withdrawls
Rule: Withdrawls must have a positive amount and sufficient funds
	
	Scenario: Withdrawl with positive amount and sufficient funds
		Given I have an account with Bank4Us with a balance of 200
		And I want to withdraw an amount less than or equal to my balance
		When I make the withdrawl
		Then The withdrawl should be approved
	
	Scenario: Withdrawl with insufficient funds
		Given I have an account with Bank4Us with a balance of 200
		And I want to withdraw an amount greater than my balance
		When I make the withdrawl
		Then The withdrawl should be cancelled

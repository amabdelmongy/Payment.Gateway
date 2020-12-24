# Payment Gateway

## Background
E-Commerce is experiencing exponential growth and merchants who sell their goods or services online
need a way to easily collect money from their customers.
We would like to build a payment gateway, an API based application that will allow a merchant to offer a
way for their shoppers to pay for their product.

Processing a payment online involves multiple steps and entities:
1. Shopper: Individual who is buying the product online.
2. Merchant: The seller of the product. For example, Apple or Amazon.
3. Payment Gateway: Responsible for validating requests, storing card information and forwarding
payment requests and accepting payment responses to and from the acquiring bank.
4. Acquiring Bank: Allows us to do the actual retrieval of money from the shopper’s card and payout to the
merchant. It also performs some validation of the card information and then sends the payment details
to the appropriate 3rd party organization for processing.
We will be building the payment gateway only and simulating the acquiring bank component in order to
allow us to fully test the payment flow.

## Requirements
The product requirements for this initial phase are the following:
1. A merchant should be able to process a payment through the payment gateway and receive either a
successful or unsuccessful response
2. A merchant should be able to retrieve the details of a previously made payment

## Process a payment
The payment gateway will need to provide merchants with a way to process a payment. To do this, the
merchant should be able to submit a request to the payment gateway. A payment request should include
appropriate fields such as the card number, expiry month/date, amount, currency, and cvv.

#### Note: Simulating the bank
In this solution simulate and mock out the Bank part of processing flow. This component should be able to be switched out for a real bank once we move into production. We should assume that a bank response returns a unique identifier and a status that indicates whether the payment was successful.

## Retrieving a payment’s details
The second requirement for the payment gateway is to allow a merchant to retrieve details of a
previously made payment using its identifier. Doing this will help the merchant with their reconciliation
and reporting needs. The response should include a masked card number and card details along with a
status code which indicates the result of the payment.

## Deliverables
1. Build an API that allows a merchant to  
  • Process a payment through your payment gateway.  
  • Retrieve details of a previously made payment.
2. Build a simulator to mock the responses from the bank to test the API from your first deliverable.

##  Description

A simple web api. It consists of a backend only.

## Solution Description
 
## Target Frameworks
Target frameworks: .NET core 5.0

## Technology
 - C#
 - .Net core
 - Swagger
 - NUnit
 - Moq
 - Dapper

## Architecture
### CQRS
It clears that we need two paths one path to write and other path to read. so CQRS is best soluation.
CQRS stands for Command Query Responsibility Segregation. We use a different model to update information than the model you use to read information.

### EventSourcing
Capture all changes to an application state as a sequence of events.
Event Sourcing ensures that all changes to application state are stored as a sequence of events.  
1- we query these events,  
2- we can also use the event log to reconstruct past states, and as a foundation to automatically adjust the state to cope with retroactive changes.

### Onion architecture
The Onion Architecture is an Architectural Pattern that enables maintainable and evolutionary enterprise systems.

##  How to run the code
 - Build with Visiual studio 2019 and run

## Deliverables  
1. Build an API that allows a merchant to  
    A. Process a payment through your payment gateway.  
      Using API url Post action ```/api/v1/payment/request-payment``` 
        Input Dto like
        ```
        {
          "MerchantId": "21220994-fc2a-43ed-bc80-2f42990ae3ac",
          "Amount": {
              "Value": 10,
              "Currency": "Euro"
          },
          "Card": {
              "Number": "5105105105105100",
              "Expiry": "9/24",
              "Cvv": "123"

          }
        }
        ```
        • In case of Accepted result will be like :-
        Http status 200 Ok
        ```
        {
          "PaymentId": "e075b909-1971-4e83-a4e5-c4c79eb09501",
          "MerchantId": "21220994-fc2a-43ed-bc80-2f42990ae3ac",
          "AcquiringBankId": "a0670fc8-9f70-47e6-8214-45eb8cc36eea"
        }
        ```
        PaymentId:- is id generating by our gateway system and it's unique for every transaction.
        AcquiringBankId:- is id coming from bank unique for every transaction.
        MerchantId:- is id comming from request


        • In case of Rejected result will be like :-
        Http status 400 Bad Request
        ```
        {
          "subject": "Rejected to acquiring Bank with Payment Id 6e45835c-0b0e-4ab7-9141-46a406ef1b7d",
          "message": "Card is not valid."
        }
        ```
        • In case of Bank throw exception result will be like :-
        Http status 400 Bad Request
          {
              "subject": "Failed calling Acquiring Bank",
              "message": "something modified at Acquiring Bank Service."
          }

    B. Retrieve details of a previously made payment.  
      ```
      api/v1/payment-details/{merchant-id}/get-by-merchant-id
      ```

2. Build a simulator to mock the responses from the bank to test the API from your first deliverable.
    ### Acquiring Bank Mock returns  
      • Accepted Result :-  
        payment succeeds to transfer money from Card to Merchant. It retuen succeeds with bank result Id.

      • Throw Exception :-  
        for example some unplanned changes happened on the Bankside.  

      • Rejected Card :-  
        for example Bank rejects the transfer because the card invalid. It retuen Rejected with bank result Id.  

      • Rejected Amount :-  
        for example Bank rejects the transfer because the card amount is not enough. It retuen Rejected with bank result Id.  

## ToDo
1. Added more unit tests.
2. Replace Inmemory Service bus by any message bus service like Azure service bus.
3. Using graphql at Api controller.

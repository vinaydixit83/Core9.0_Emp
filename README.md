# Core9.0_Emp
.Net core 8.0 Employee System

1. It has docker file. Which will be used to run docker on your local and Create Image and Containers.
2. It has rabbitMq code. When an order will be submitted by client.
3.  IT will put the push the order into RabbitMq as producer by using Queuename, Exchange name and Routing key basis.
4. The other Consumer will subscribe the RabbitMq by using the same Queuename, Exchange name and Routing and grab the detail from Queue.
5. Then ORder Service again will be used as producer and Put order Created detail into Queue.
6. The payment service again subscribe it.

 Sequence: 
 Order Submitted:
1. Client submits an order.
Order Service publishes Order_Created.
Order Processed by Payment:

2. Payment Service consumes Order_Created.
Payment is processed, and Payment_Processed or Payment_Failed is published.

3. Shipment Created:
Shipment Service consumes Payment_Processed.
Shipment is created, and Shipment_Created is published.

# Stripe Payment Integration - Ra7ala API

## Overview
This document outlines the complete Stripe payment integration implemented in the Ra7ala API project. The integration includes payment processing, webhook handling, and ticket generation upon successful payment.

## Configuration Required

### 1. Stripe API Keys (Already Added)
Add the following to your `appsettings.json`:

```json
"Stripe": {
  "PublishableKey": " ",
  "SecretKey": " ",
  "WebhookSecret": " ",
  "Currency": "usd",
  "SuccessUrl": "http://localhost:4200/payment/success",
  "CancelUrl": "http://localhost:4200/payment/cancel"
}
```

### 2. Stripe CLI Setup
To receive webhooks locally, run:
```bash
stripe listen --forward-to https://localhost:7111/api/payments/webhook -e payment_intent.succeeded,payment_intent.payment_failed
```

## Files Created/Modified

### New Files Created:
1. **`Application/Models/StripeSettings.cs`** - Stripe configuration model
2. **`Application/DTOs/Payment/PaymentDTOs.cs`** - Payment-related DTOs
3. **`Application/Services.Interfaces/IPaymentService.cs`** - Payment service interface
4. **`Infrastructure/ExternalServices/PaymentService/PaymentService.cs`** - Stripe payment implementation
5. **`Presentation/Controllers/PaymentsController.cs`** - Payment API endpoints
6. **`Presentation/Extensions/PaymentServiceExtension.cs`** - Service registration

### Modified Files:
1. **`Application/DTOs/Booking/BookingDTOs.cs`** - Enhanced with payment fields
2. **`Application/Services/Booking/BookingService.cs`** - Integrated with payment service
3. **`Presentation/Program.cs`** - Added payment service registration
4. **`Presentation/Errors/ApiResponse.cs`** - Added constructor for data responses
5. **`Presentation/appsettings.json`** - Added Stripe configuration

## API Endpoints

### Payment Endpoints:

1. **POST `/api/payments/create-payment-intent`**
   - Creates a Stripe payment intent
   - Requires: BookingId, Amount, Currency
   - Returns: ClientSecret for frontend payment

2. **POST `/api/payments/confirm-payment`**
   - Confirms payment and generates tickets
   - Requires: PaymentIntentId, BookingId
   - Returns: Payment status and ticket information

3. **GET `/api/payments/status/{paymentIntentId}`**
   - Gets payment status from Stripe
   - Returns: Payment details and status

4. **POST `/api/payments/refund`**
   - Processes refunds (Admin/SuperAdmin only)
   - Requires: PaymentIntentId, Amount (optional)
   - Returns: Refund details

5. **POST `/api/payments/webhook`**
   - Handles Stripe webhook events
   - Automatically processes successful payments
   - Generates tickets on payment success

6. **GET `/api/payments/config`**
   - Returns Stripe publishable key for frontend
   - Used by frontend to initialize Stripe

## Payment Flow

### 1. Create Booking
- Passenger creates a booking (status: Pending)
- Booking is not paid initially

### 2. Process Payment
- Call `/api/bookings/process-payment` with BookingId
- System creates Stripe PaymentIntent
- Returns ClientSecret for frontend payment

### 3. Frontend Payment
- Frontend uses Stripe.js with ClientSecret
- User completes payment on Stripe's secure form

### 4. Payment Confirmation
- Webhook automatically processes successful payments
- OR manually call `/api/payments/confirm-payment`
- Booking status updated to Confirmed
- Tickets generated automatically

### 5. Ticket Generation
- Tickets created with unique TicketCode
- Price distributed evenly across tickets
- QR codes can be added for validation

## Key Features

### 1. Security
- Webhook signature validation
- Role-based access control
- Secure API key handling

### 2. Error Handling
- Comprehensive error responses
- Stripe exception handling
- Transaction rollback on failures

### 3. Webhooks
- Automatic payment processing
- Event-driven architecture
- Handles payment success/failure

### 4. Ticket Management
- Automatic ticket generation
- Unique ticket codes
- Seat management integration

## Testing

### 1. Test Cards (Stripe Test Mode)
- Success: `4242424242424242`
- Declined: `4000000000000002`
- Requires Authentication: `4000002500003155`

### 2. Webhook Testing
```bash
# Start webhook forwarding
stripe listen --forward-to https://localhost:7111/api/payments/webhook

# Test webhook
stripe trigger payment_intent.succeeded
```

### 3. API Testing
Use the provided endpoints with Postman or similar tools to test payment flows.

## Frontend Integration Guide

### 1. Get Stripe Configuration
```javascript
const config = await fetch('/api/payments/config');
const { publishableKey } = await config.json();
```

### 2. Initialize Stripe
```javascript
const stripe = Stripe(publishableKey);
```

### 3. Create Payment Intent
```javascript
const response = await fetch('/api/payments/create-payment-intent', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    bookingId: bookingId,
    amount: amount,
    currency: 'usd'
  })
});
const { clientSecret } = await response.json();
```

### 4. Process Payment
```javascript
const { error, paymentIntent } = await stripe.confirmCardPayment(clientSecret, {
  payment_method: {
    card: cardElement,
    billing_details: { name: customerName }
  }
});

if (!error && paymentIntent.status === 'succeeded') {
  // Payment successful - tickets generated automatically via webhook
}
```

## Dependencies Added
- **Stripe.NET** v48.2.0 (Application & Infrastructure projects)

## Database Changes
No database schema changes were required. The existing Ticket and Booking entities support the payment integration.

## Security Considerations
1. Never expose secret keys to frontend
2. Always validate webhook signatures
3. Use HTTPS for production webhooks
4. Implement proper authentication for payment endpoints
5. Store sensitive data securely

## Deployment Notes
1. Update webhook endpoint URL for production
2. Use production Stripe keys
3. Configure proper CORS settings
4. Set up proper SSL certificates
5. Monitor webhook delivery in Stripe Dashboard

This integration provides a complete, production-ready payment system with Stripe for the Ra7ala transportation platform.

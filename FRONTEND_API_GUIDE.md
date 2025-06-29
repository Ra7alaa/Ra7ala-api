# Ra7ala API - Frontend Integration Guide
## Booking Process & Ticket Generation

This document provides complete API endpoints and workflows for implementing the booking process and ticket generation in the Ra7ala frontend application.

## Authentication Required
All booking endpoints require JWT Bearer authentication with `Passenger` role.

```javascript
// Add Authorization header to all requests
headers: {
  'Authorization': `Bearer ${userToken}`,
  'Content-Type': 'application/json'
}
```

---

## 🎯 Complete Booking Flow

### Step 1: Search for Available Trips

**Endpoint:** `POST /api/trips/search`

**Purpose:** Find available trips based on user criteria

**Request Body:**
```json
{
  "startCityId": 1,
  "endCityId": 2,
  "departureDate": "2025-07-15T08:00:00Z",
  "requiredSeats": 2
}
```

**Response:**
```json
{
  "statusCode": 200,
  "message": "Trips search completed successfully",
  "data": [
    {
      "id": 123,
      "routeName": "Cairo to Alexandria",
      "departureTime": "2025-07-15T08:00:00Z",
      "arrivalTime": "2025-07-15T11:30:00Z",
      "availableSeats": 25,
      "price": 75.00,
      "companyName": "Express Bus Co",
      "busRegistrationNumber": "ABC-1234",
      "amenityDescription": "AC, WiFi, USB Charging",
      "tripStations": [
        {
          "stationName": "Cairo Central",
          "cityName": "Cairo",
          "arrivalTime": "2025-07-15T08:00:00Z",
          "departureTime": "2025-07-15T08:00:00Z",
          "stopOrder": 1
        }
      ]
    }
  ]
}
```

### Step 2: Create Booking

**Endpoint:** `POST /api/booking`

**Purpose:** Create a new booking (status: Pending, not paid)

**Request Body:**
```json
{
  "tripId": 123,
  "startStationId": 1,
  "endStationId": 5,
  "numberOfTickets": 2
}
```

**Response:**
```json
{
  "statusCode": 200,
  "message": "Booking created successfully",
  "data": {
    "id": 456,
    "passengerId": "user123",
    "passengerName": "John Doe",
    "tripId": 123,
    "startStationName": "Cairo Central",
    "startCityName": "Cairo",
    "endStationName": "Alexandria Main",
    "endCityName": "Alexandria",
    "bookingDate": "2025-06-29T10:30:00Z",
    "totalPrice": 150.00,
    "status": "Pending",
    "isPaid": false,
    "numberOfTickets": 2,
    "tickets": []
  }
}
```

### Step 3: Process Payment

**Endpoint:** `POST /api/booking/payment`

**Purpose:** Create Stripe payment intent and optionally process payment

**Request Body:**
```json
{
  "bookingId": 456,
  "paymentMethod": "CreditCard",
  "paymentMethodToken": null,
  "customerEmail": "john@example.com",
  "customerName": "John Doe"
}
```

**Response:**
```json
{
  "statusCode": 200,
  "message": "Payment intent created. Please complete payment using the client secret.",
  "data": {
    "bookingId": 456,
    "success": false,
    "totalPrice": 150.00,
    "paymentStatus": "requires_payment_method",
    "paymentIntentId": "pi_3ABC123def456GHI",
    "clientSecret": "pi_3ABC123def456GHI_secret_xyz789",
    "tickets": [],
    "message": "Payment intent created. Please complete payment using the client secret."
  }
}
```

### Step 4: Complete Payment with Stripe (Frontend)

Use the `clientSecret` from Step 3 to complete payment with Stripe Elements:

```javascript
// Initialize Stripe
const stripe = Stripe('Publish_Key_here');

// Confirm payment
const { error, paymentIntent } = await stripe.confirmCardPayment(clientSecret, {
  payment_method: {
    card: cardElement,
    billing_details: {
      name: 'John Doe',
      email: 'john@example.com'
    }
  }
});

if (!error && paymentIntent.status === 'succeeded') {
  // Payment successful - tickets generated automatically via webhook
  console.log('Payment successful!');
  // Redirect to tickets page or show success message
}
```

### Step 5: Verify Payment & Get Tickets

**Option A: Check Payment Status**

**Endpoint:** `GET /api/payments/status/{paymentIntentId}`

```json
{
  "statusCode": 200,
  "message": "Payment status retrieved successfully",
  "data": {
    "isSuccess": true,
    "paymentIntentId": "pi_3ABC123def456GHI",
    "status": "succeeded",
    "amount": 150.00,
    "currency": "usd",
    "paymentDate": "2025-06-29T10:35:00Z",
    "bookingId": 456
  }
}
```

**Option B: Get Updated Booking**

**Endpoint:** `GET /api/booking/{bookingId}`

```json
{
  "statusCode": 200,
  "message": "Booking retrieved successfully",
  "data": {
    "id": 456,
    "status": "Confirmed",
    "isPaid": true,
    "totalPrice": 150.00,
    "numberOfTickets": 2,
    "tickets": [
      {
        "id": 789,
        "tripId": 123,
        "bookingId": 456,
        "passengerId": "user123",
        "passengerName": "John Doe",
        "seatNumber": null,
        "price": 75.00,
        "purchaseDate": "2025-06-29T10:35:00Z",
        "isUsed": false,
        "ticketCode": "TKT202506291035001234"
      },
      {
        "id": 790,
        "tripId": 123,
        "bookingId": 456,
        "passengerId": "user123",
        "passengerName": "John Doe",
        "seatNumber": null,
        "price": 75.00,
        "purchaseDate": "2025-06-29T10:35:00Z",
        "isUsed": false,
        "ticketCode": "TKT202506291035005678"
      }
    ]
  }
}
```

---

## 📋 Additional Booking Management Endpoints

### Get All User Bookings

**Endpoint:** `GET /api/booking/my-bookings?pageNumber=1&pageSize=10`

**Response:**
```json
{
  "statusCode": 200,
  "message": "Bookings retrieved successfully",
  "data": {
    "totalCount": 5,
    "pageNumber": 1,
    "pageSize": 10,
    "bookings": [
      {
        "id": 456,
        "tripId": 123,
        "startStationName": "Cairo Central",
        "endStationName": "Alexandria Main",
        "bookingDate": "2025-06-29T10:30:00Z",
        "totalPrice": 150.00,
        "status": "Confirmed",
        "isPaid": true,
        "numberOfTickets": 2
      }
    ]
  }
}
```

### Get All User Tickets

**Endpoint:** `GET /api/booking/my-tickets?pageNumber=1&pageSize=10`

**Response:**
```json
{
  "statusCode": 200,
  "message": "Tickets retrieved successfully",
  "data": {
    "totalCount": 4,
    "pageNumber": 1,
    "pageSize": 10,
    "tickets": [
      {
        "id": 789,
        "tripId": 123,
        "bookingId": 456,
        "ticketCode": "TKT202506291035001234",
        "price": 75.00,
        "purchaseDate": "2025-06-29T10:35:00Z",
        "isUsed": false
      }
    ]
  }
}
```

### Cancel Booking

**Endpoint:** `POST /api/booking/cancel/{bookingId}`

**Response:**
```json
{
  "statusCode": 200,
  "message": "Booking cancelled successfully"
}
```

---

## 🎟️ Payment-Only Endpoints

### Get Stripe Configuration

**Endpoint:** `GET /api/payments/config`

**Response:**
```json
{
  "statusCode": 200,
  "message": "Stripe configuration retrieved successfully",
  "data": {
    "publishableKey": "publishable_Key_here",
    "currency": "usd"
  }
}
```

### Create Payment Intent Directly

**Endpoint:** `POST /api/payments/create-payment-intent`

**Request Body:**
```json
{
  "bookingId": 456,
  "amount": 150.00,
  "currency": "usd",
  "description": "Ra7ala Trip Booking #456",
  "customerEmail": "john@example.com"
}
```

### Confirm Payment

**Endpoint:** `POST /api/payments/confirm-payment`

**Request Body:**
```json
{
  "paymentIntentId": "pi_3ABC123def456GHI",
  "bookingId": 456
}
```

---

## 🔄 Complete Frontend Workflow Example

```javascript
class BookingService {
  
  async searchTrips(searchCriteria) {
    const response = await fetch('/api/trips/search', {
      method: 'POST',
      headers: this.getHeaders(),
      body: JSON.stringify(searchCriteria)
    });
    return response.json();
  }
  
  async createBooking(bookingData) {
    const response = await fetch('/api/booking', {
      method: 'POST',
      headers: this.getHeaders(),
      body: JSON.stringify(bookingData)
    });
    return response.json();
  }
  
  async processPayment(paymentData) {
    const response = await fetch('/api/booking/payment', {
      method: 'POST',
      headers: this.getHeaders(),
      body: JSON.stringify(paymentData)
    });
    return response.json();
  }
  
  async completeStripePayment(clientSecret, cardElement) {
    const stripe = Stripe(this.stripePublishableKey);
    
    const { error, paymentIntent } = await stripe.confirmCardPayment(clientSecret, {
      payment_method: {
        card: cardElement,
        billing_details: {
          name: this.currentUser.name,
          email: this.currentUser.email
        }
      }
    });
    
    return { error, paymentIntent };
  }
  
  async getBookingWithTickets(bookingId) {
    const response = await fetch(`/api/booking/${bookingId}`, {
      headers: this.getHeaders()
    });
    return response.json();
  }
  
  async getMyTickets(pageNumber = 1, pageSize = 10) {
    const response = await fetch(`/api/booking/my-tickets?pageNumber=${pageNumber}&pageSize=${pageSize}`, {
      headers: this.getHeaders()
    });
    return response.json();
  }
  
  getHeaders() {
    return {
      'Authorization': `Bearer ${this.userToken}`,
      'Content-Type': 'application/json'
    };
  }
}
```

---

## 🎯 Key Points for Frontend Implementation

### 1. **Booking Status Flow**
- `Pending` → Initial booking creation
- `Confirmed` → After successful payment
- `Cancelled` → User cancelled or payment failed

### 2. **Payment Integration**
- Use provided Stripe publishable key
- Handle payment confirmation client-side
- Tickets are generated automatically via webhooks
- Always verify final status after payment

### 3. **Error Handling**
- Check `statusCode` in response
- Handle validation errors from `message`
- Implement retry logic for payment failures

### 4. **Security**
- Always include JWT token in requests
- Never expose secret keys in frontend
- Validate user permissions on sensitive operations

### 5. **User Experience**
- Show loading states during payment processing
- Provide clear feedback for each step
- Allow users to view booking history and tickets
- Implement proper error messages

This guide provides everything needed to implement the complete booking and ticketing flow in your frontend application.

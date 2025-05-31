#!/bin/bash

echo "🚀 Testing E-Commerce API..."

API_URL="http://localhost:5000"

# Test basic connectivity
echo "📡 Testing API connectivity..."
STATUS=$(curl -s -o /dev/null -w "%{http_code}" $API_URL/api/products)

if [ "$STATUS" -eq 200 ]; then
    echo "✅ API is running and responding on $API_URL"
    
    # Test getting products (no auth required)
    echo "📦 Testing products endpoint..."
    PRODUCTS=$(curl -s $API_URL/api/products | jq '. | length' 2>/dev/null)
    
    if [ $? -eq 0 ]; then
        echo "✅ Products endpoint working - Found $PRODUCTS products"
    else
        echo "⚠️  Products endpoint working but jq not available for parsing"
    fi
    
    echo ""
    echo "🎯 Ready for Postman testing!"
    echo "📝 Import the collection from: .postman/ecommerce-api.postman_collection.json"
    echo "🌍 Set environment variable: ecommerceurl = $API_URL"
    echo ""
    echo "🔐 Start by running one of the login requests in the Auth folder"
    echo "📋 Then test the new Address Management endpoints!"
    
else
    echo "❌ API is not responding. Status code: $STATUS"
    echo "🔧 Make sure the API is running with: dotnet run --project ECommerceAPI"
fi 
# Test the Todo API

## Testing with curl (after authentication setup)

Replace `YOUR_ACCESS_TOKEN` with a valid JWT token from Microsoft Entra ID.

### Get all todos

```bash
curl -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
     -H "Content-Type: application/json" \
     https://localhost:5001/api/todos
```

### Create a new todo

```bash
curl -X POST \
     -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
     -H "Content-Type: application/json" \
     -d '{"title":"Test Todo","description":"Testing the API","isCompleted":false}' \
     https://localhost:5001/api/todos
```

### Get a specific todo (replace {id} with actual ID)

```bash
curl -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
     -H "Content-Type: application/json" \
     https://localhost:5001/api/todos/{id}
```

### Update a todo (replace {id} with actual ID)

```bash
curl -X PUT \
     -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
     -H "Content-Type: application/json" \
     -d '{"id":{id},"title":"Updated Todo","description":"Updated description","isCompleted":true}' \
     https://localhost:5001/api/todos/{id}
```

### Delete a todo (replace {id} with actual ID)

```bash
curl -X DELETE \
     -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
     -H "Content-Type: application/json" \
     https://localhost:5001/api/todos/{id}
```

## Notes

- Replace `localhost:5001` with your actual server URL
- All endpoints require authentication
- The API returns JSON responses
- User can only access their own todos based on the JWT token's user ID

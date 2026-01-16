# Map Library Integration - Leaflet with Routing

## Overview

The application now uses **Leaflet** (open-source mapping library) with **Leaflet Routing Machine** for displaying fuel stations on an interactive map and providing turn-by-turn directions to the nearest station.

## Library Details

### Leaflet
- **Type**: Open-source JavaScript library
- **Website**: https://leafletjs.com/
- **License**: BSD 2-Clause (free for commercial use)
- **CDN**: Included via unpkg.com CDN
- **Features**: 
  - Interactive maps
  - Markers and popups
  - Custom icons
  - Mobile-friendly

### Leaflet Routing Machine
- **Type**: Plugin for Leaflet
- **Website**: https://www.liedman.net/leaflet-routing-machine/
- **License**: ISC (free for commercial use)
- **CDN**: Included via unpkg.com CDN
- **Features**:
  - Turn-by-turn directions
  - Route visualization
  - Distance and time calculations

### Routing Backend: OSRM (Open Source Routing Machine)
- **Type**: Free routing service
- **Service URL**: https://router.project-osrm.org/
- **License**: BSD 2-Clause
- **Limitations**: 
  - Public instance has rate limits
  - For production, consider self-hosting or using a commercial service
- **Alternatives**:
  - GraphHopper (self-hosted or commercial)
  - Mapbox Directions API (commercial)
  - Google Maps Directions API (commercial)

## Features Implemented

1. **Interactive Map Display**
   - Shows all fuel stations with coordinates
   - Color-coded markers (green=open, red=closed, orange=offloading)
   - Station popups with basic information

2. **User Location Detection**
   - "Use My Location" button uses browser geolocation API
   - Automatically finds nearest station
   - Displays user location marker on map

3. **Directions & Routing**
   - Calculates route from user location to selected station
   - Shows route line on map
   - Displays distance and estimated travel time
   - Turn-by-turn directions panel

4. **Station Filtering**
   - Filter by fuel type
   - Shows only stations with available stock
   - Updates map markers dynamically

5. **Station List Sidebar**
   - Lists all stations with status badges
   - Quick "Get Directions" buttons
   - Scrollable list for many stations

## Usage

### For Users:
1. Navigate to "Fuel Availability" → "Map & Directions"
2. Click "Use My Location" to detect current position
3. Select a station from the dropdown or click "Get Directions" on any station card
4. View the route on the map with distance and time estimates

### For Developers:

#### Adding More Routing Options:
You can switch to different routing services by modifying the router configuration in `Map.cshtml`:

```javascript
// Current: OSRM (free)
router: L.Routing.osrmv1({
    serviceUrl: 'https://router.project-osrm.org/route/v1'
})

// Alternative: GraphHopper (requires API key)
router: L.Routing.graphHopper('YOUR_API_KEY', {
    urlParameters: {
        vehicle: 'car'
    }
})

// Alternative: Mapbox (requires access token)
router: L.Routing.mapbox('YOUR_ACCESS_TOKEN')
```

#### Customizing Map Tiles:
You can change the map tile provider by modifying the tile layer:

```javascript
// Current: OpenStreetMap (free)
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {...})

// Alternative: Mapbox (requires access token)
L.tileLayer('https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token={accessToken}', {
    id: 'mapbox/streets-v11',
    accessToken: 'YOUR_ACCESS_TOKEN'
})
```

## Cost Considerations

### Free Options (Current Implementation):
- ✅ Leaflet library: Free
- ✅ Leaflet Routing Machine: Free
- ✅ OpenStreetMap tiles: Free
- ✅ OSRM public routing: Free (with rate limits)

### Commercial Alternatives:
- **Google Maps Platform**: ~$200/month for moderate usage
- **Mapbox**: ~$50-200/month depending on usage
- **GraphHopper Cloud**: ~$50-500/month
- **Self-hosted OSRM/GraphHopper**: Server costs only

## Production Recommendations

1. **For Low-Medium Traffic**: Current setup (OSRM public) is fine
2. **For High Traffic**: 
   - Self-host OSRM or GraphHopper
   - Or use commercial service (Mapbox/Google)
3. **For Better Coverage**: Consider Mapbox or Google Maps for Zimbabwe-specific routing data
4. **For Offline Support**: Self-host routing engine with local OSM data

## Browser Compatibility

- ✅ Chrome/Edge (latest)
- ✅ Firefox (latest)
- ✅ Safari (latest)
- ✅ Mobile browsers (iOS Safari, Chrome Mobile)
- ⚠️ IE11: Not supported (Leaflet requires modern browser)

## Security Notes

- Geolocation API requires HTTPS in production (or localhost for development)
- User location is only used client-side, not stored
- No API keys required for current implementation
- OSRM public service is rate-limited to prevent abuse

## Future Enhancements

1. **Multiple Route Options**: Show alternative routes
2. **Traffic-Aware Routing**: Integrate traffic data (requires commercial service)
3. **Offline Maps**: Cache map tiles for offline use
4. **Route Optimization**: For multiple stations
5. **Real-time Traffic**: Show current traffic conditions
6. **Mobile App Integration**: Use same routing logic in mobile app


import { useEffect, useRef, useState } from 'react'
import L, { LatLng } from 'leaflet'
import 'leaflet/dist/leaflet.css'
import { fetchRoundTrip, exportGpx } from '../lib/api'


// Fix default marker icons in Vite
// @ts-ignore
import markerIcon2x from 'leaflet/dist/images/marker-icon-2x.png'
// @ts-ignore
import markerIcon from 'leaflet/dist/images/marker-icon.png'
// @ts-ignore
import markerShadow from 'leaflet/dist/images/marker-shadow.png'


L.Icon.Default.mergeOptions({
iconRetinaUrl: markerIcon2x,
iconUrl: markerIcon,
shadowUrl: markerShadow,
})


interface Props {
km: number
mode: 'foot'|'bike'
seed?: number
}


export default function MapView({ km, mode, seed }: Props) {
const mapRef = useRef<HTMLDivElement | null>(null)
const [map, setMap] = useState<L.Map | null>(null)
const [start, setStart] = useState<LatLng | null>(null)
const [routeLayer, setRouteLayer] = useState<L.Polyline | null>(null)
const [info, setInfo] = useState('')
const [error, setError] = useState<string | null>(null)


useEffect(() => {
if (!mapRef.current || map) return
const m = L.map(mapRef.current).setView([57.7089, 11.9746], 13)
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 19 }).addTo(m)


m.on('click', (e: any) => setStart(e.latlng))
setMap(m)
}, [map])


useEffect(() => {
	if (!map || !start) return;
	const marker = L.marker(start, { draggable: true }).addTo(map);
	marker.on('dragend', () => setStart(marker.getLatLng()));
	return () => {
		marker.remove();
	};
}, [map, start]);


async function useLocation() {
if (!navigator.geolocation) return alert('Geolokalisering stöds inte')
navigator.geolocation.getCurrentPosition(
p => setStart(L.latLng(p.coords.latitude, p.coords.longitude)),
e => alert('Kunde inte hämta plats: ' + e.message),
{ enableHighAccuracy: true, timeout: 10000 }
)
}


async function generate() {
if (!map || !start) { 
alert('Välj startpunkt först'); 
return 
}
setError(null)
setInfo('Beräknar rutt…')


try {
const data = await fetchRoundTrip(start.lat, start.lng, km, mode, seed)
const coords = data.coordinates // [lat,lng]


routeLayer?.remove()
const rl = L.polyline(coords.map(([lat, lng]: [number, number]) => [lat, lng]), { weight: 5 }).addTo(map)
setRouteLayer(rl)
map.fitBounds(rl.getBounds(), { padding: [30, 30] })


const distKm = (data.distanceMeters / 1000).toFixed(2)
const durMin = Math.round(data.durationMs / 60000)
setInfo(`~${distKm} km • ~${durMin} min`)
} catch (e: any) {
console.error(e)
setError(e.message || 'Kunde inte generera rutt')
setInfo('')
}
}


return (
<div>
<div ref={mapRef} style={{ height: 400, width: '100%' }} />
<div style={{ marginTop: 10 }}>
<button onClick={useLocation}>Använd min plats</button>
<button onClick={generate}>Generera rutt</button>
{info && <span style={{ marginLeft: 10 }}>{info}</span>}
{error && <div style={{ color: 'red' }}>{error}</div>}
</div>
</div>
)
}
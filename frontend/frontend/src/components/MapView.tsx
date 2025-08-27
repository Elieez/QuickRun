import { useEffect, useRef, useState } from 'react'
import L, { LatLng } from 'leaflet'
import 'leaflet/dist/leaflet.css'
import { fetchRoundTrip } from '../lib/api'

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
  mode: 'foot' | 'bike'
  seed?: number
}

export default function MapView({ km, mode, seed }: Props) {
  // Keep DOM container and map instance separate
  const containerRef = useRef<HTMLDivElement | null>(null)
  const mapRef = useRef<L.Map | null>(null)

  const [start, setStart] = useState<LatLng | null>(null)
  const markerRef = useRef<L.Marker | null>(null)
  const routeRef = useRef<L.Polyline | null>(null)

  const [info, setInfo] = useState('')
  const [error, setError] = useState<string | null>(null)

  // Init map once
  useEffect(() => {
    if (!containerRef.current || mapRef.current) return

    const m = L.map(containerRef.current).setView([57.7089, 11.9746], 13)
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 19 }).addTo(m)
    m.on('click', (e: any) => setStart(e.latlng))
    mapRef.current = m

    // Dispose on unmount (fixes double-init in React StrictMode)
    return () => {
      m.off()
      m.remove()
      mapRef.current = null
    }
  }, [])

  // Start marker lifecycle
  useEffect(() => {
    const map = mapRef.current
    if (!map || !start) return

    // remove old marker
    if (markerRef.current) {
      markerRef.current.remove()
      markerRef.current = null
    }

    const marker = L.marker(start, { draggable: true }).addTo(map)
    marker.on('dragend', () => setStart(marker.getLatLng()))
    markerRef.current = marker

    return () => {
      marker.remove()
      if (markerRef.current === marker) markerRef.current = null
    }
  }, [start])

  async function useLocation() {
    if (!navigator.geolocation) return alert('Geolokalisering stöds inte')
    navigator.geolocation.getCurrentPosition(
      (p) => setStart(L.latLng(p.coords.latitude, p.coords.longitude)),
      (e) => alert('Kunde inte hämta plats: ' + e.message),
      { enableHighAccuracy: true, timeout: 10000 }
    )
  }

  async function generate() {
    const map = mapRef.current
    if (!map || !start) {
      alert('Välj startpunkt först')
      return
    }

    setError(null)
    setInfo('Beräknar rutt…')

    try {
      const data = await fetchRoundTrip(start.lat, start.lng, km, mode, seed)
      const coords = data.coordinates // [lat, lng]

      // remove previous route polyline
      if (routeRef.current) {
        routeRef.current.remove()
        routeRef.current = null
      }

      const rl = L.polyline(coords.map(([lat, lng]) => [lat, lng]), { weight: 5 }).addTo(map)
      routeRef.current = rl
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
    <div style={{ height: '100%', width: '100%', position: 'relative' }}>
        <div ref={containerRef} className="map" />
        <div style={{ position: 'absolute', bottom: 12, left: 12, zIndex: 1000 }}>
            <button onClick={useLocation}>Använd min plats</button>
            <button onClick={generate} style={{ marginLeft: 8 }}>Generera rutt</button>
            {info && <span style={{ marginLeft: 10 }}>{info}</span>}
            {error && <div className="error">{error}</div>}
        </div>
    </div>
  )
}

import type { RoundTripResponse } from './types'

const BASE = import.meta.env.VITE_API_BASE ?? '/api'

export async function fetchRoundTrip(lat: number, lng: number, km: number, mode: 'foot'|'bike', seed?: number): Promise<RoundTripResponse> {
    const url = new URL(`${BASE}/roundtrip`, window.location.origin)
    url.searchParams.set('lat', String(lat))
    url.searchParams.set('lng', String(lng))
    url.searchParams.set('km', String(km))
    url.searchParams.set('mode', mode)
    if (seed !== undefined) url.searchParams.set('seed', String(seed))

    const res = await fetch(url.toString().replace(window.location.origin, ''))
    if (!res.ok) {
        let message = `Roundtrip failed: ${res.status}`
        try {
            const err = await res.json()
            if (err?.detail) message = err.detail
        } catch {
            // ignore
        }
        throw new Error(message)
    }
    return await res.json()
}

export async function exportGpx(coords: [number, number][], name: string) {
    const res = await fetch(`${BASE}/gpx`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ coordinates: coords, name })
    })
    if (!res.ok) throw new Error(`GPX export failed: ${res.status}`)
    const gpx = await res.text()
    const blob = new Blob([gpx], { type: 'application/gpx+xml' })
    const a = document.createElement('a')
    a.href = URL.createObjectURL(blob)
    a.download = `${name || 'route'}.gpx`
    document.body.appendChild(a)
    a.click()
    URL.revokeObjectURL(a.href)
    a.remove()
}
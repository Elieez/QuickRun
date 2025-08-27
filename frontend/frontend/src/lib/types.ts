export type RoundTripResponse = {
    distanceMeters: number
    durationMs: number
    bbox: number[]
    coordinates: [number, number][] // [lat,lng]
    raw: unknown
}
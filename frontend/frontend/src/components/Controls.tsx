import React from 'react'

type Props = {
    km: number
    setKm: (v: number) => void
    mode: 'foot'|'bike'
    setMode: (v: 'foot'|'bike') => void
    seed?: number
    setSeed: (v: number | undefined) => void
}

export default function Controls({ km, setKm, mode, setMode, seed, setSeed }: Props) {
    return (
        <div className="panel">
            <div className="row">
                <label>Läge</label>
                <select value={mode} onChange={e=>setMode(e.target.value as any)}>
                    <option value="foot">Löpning</option>
                    <option value="bike">Cykel</option>
                </select>
            </div>
            <div className="row">
                <label>Distans (km)</label>
                <input type="number" min={1} step={0.5} value={km}
                       onChange={e=>setKm(Number(e.target.value))} />
            </div>
            <div className="row">
                <label>Seed</label>
                <input type="number" placeholder="valfri"
                       value={seed ?? ''}
                       onChange={e=>{
                        const v = e.target.value
                        setSeed(v === '' ? undefined : Number(v))
                        }} />
            </div>
            <div className="info">Klicka i kartan eller använd din plats för att sätta start.</div>
        </div>
    )
}
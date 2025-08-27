import { useState } from 'react'
import MapView from './components/MapView'
import Controls from './components/Controls'
import './styles.css'


export default function App() {
const [km, setKm] = useState(5)
const [mode, setMode] = useState<'foot'|'bike'>('foot')
const [seed, setSeed] = useState<number | undefined>(undefined)


return (
<div style={{height:'100vh', width:'100vw', position:'relative'}}>
<MapView km={km} mode={mode} seed={seed} />
<Controls km={km} setKm={setKm} mode={mode} setMode={setMode} seed={seed} setSeed={setSeed} />
</div>
)
}
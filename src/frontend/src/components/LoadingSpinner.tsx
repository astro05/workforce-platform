import { Spin } from 'antd'

interface Props {
  tip?: string
}

export default function LoadingSpinner({
  tip = 'Loading...',
}: Props) {
  return (
    <div style={{
      display:        'flex',
      justifyContent: 'center',
      alignItems:     'center',
      minHeight:      300,
      flexDirection:  'column',
      gap:            16,
    }}>
      <Spin size="large" />
      <span style={{ color: '#8c8c8c' }}>{tip}</span>
    </div>
  )
}
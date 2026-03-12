import { Alert, Button } from 'antd'

interface Props {
  message?:  string
  onRetry?:  () => void
}

export default function ErrorMessage({
  message = 'Something went wrong.',
  onRetry,
}: Props) {
  return (
    <Alert
      type="error"
      message="Error"
      description={message}
      showIcon
      action={
        onRetry && (
          <Button size="small" onClick={onRetry}>
            Retry
          </Button>
        )
      }
    />
  )
}
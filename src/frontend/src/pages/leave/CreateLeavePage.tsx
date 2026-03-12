import {
  Form, Input, Select, Button, DatePicker,
  Typography, Card, Space, message, Alert,
} from 'antd'
import { ArrowLeftOutlined } from '@ant-design/icons'
import {
  useQuery, useMutation, useQueryClient,
} from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { leaveApi } from '../../api/leaveRequests'
import { employeeApi } from '../../api/employees'
import type { CreateLeaveRequestDto } from '../../types'
import dayjs from 'dayjs'

const { Title, Text } = Typography
const { Option }      = Select
const { TextArea }    = Input
const { RangePicker } = DatePicker

export default function CreateLeavePage() {
  const navigate    = useNavigate()
  const queryClient = useQueryClient()
  const [form]      = Form.useForm()

  const { data: employees } = useQuery({
    queryKey: ['employees', { page: 1, pageSize: 100 }],
    queryFn:  () => employeeApi.getAll({
      page: 1, pageSize: 100, isActive: true,
    }),
  })

  const { mutate, isPending } = useMutation({
    mutationFn: (dto: CreateLeaveRequestDto) =>
      leaveApi.create(dto),
    onSuccess: () => {
      message.success('Leave request submitted')
      queryClient.invalidateQueries({
        queryKey: ['leaveRequests'],
      })
      navigate('/leave')
    },
  })

  const onFinish = (values: Record<string, unknown>) => {
    const dateRange  = values.dateRange as [unknown, unknown]
    const start      = dateRange[0] as string
    const end        = dateRange[1] as string
    const employeeId = values.employeeId as number

    const employee = employees?.items.find(
      e => e.id === employeeId)

    if (!employee) {
      message.error('Employee not found')
      return
    }

    const dto: CreateLeaveRequestDto = {
      employeeId,
      employeeName: employee.fullName,
      leaveType:    values.leaveType as string,
      startDate:    dayjs(start).format('YYYY-MM-DD'),
      endDate:      dayjs(end).format('YYYY-MM-DD'),
      reason:       values.reason as string,
    }

    mutate(dto)
  }

  return (
    <div>
      <div className="page-header">
        <Space>
          <Button
            icon={<ArrowLeftOutlined />}
            onClick={() => navigate('/leave')}
          >
            Back
          </Button>
          <Title level={3} style={{ margin: 0 }}>
            Submit Leave Request
          </Title>
        </Space>
      </div>

      <Card style={{ maxWidth: 600 }}>
        <Alert
          type="info"
          showIcon
          message="Leave Request"
          description="Submit a leave request for an employee. The request will be pending until approved or rejected."
          style={{ marginBottom: 24 }}
        />

        <Form
          form={form}
          layout="vertical"
          onFinish={onFinish}
        >
          <Form.Item
            name="employeeId"
            label="Employee"
            rules={[{
              required: true,
              message:  'Please select an employee',
            }]}
          >
            <Select
              showSearch
              placeholder="Select employee"
              optionFilterProp="children"
            >
              {employees?.items.map(e => (
                <Option key={e.id} value={e.id}>
                  {e.fullName} — {e.departmentName}
                </Option>
              ))}
            </Select>
          </Form.Item>

          <Form.Item
            name="leaveType"
            label="Leave Type"
            rules={[{
              required: true,
              message:  'Please select leave type',
            }]}
          >
            <Select placeholder="Select leave type">
              <Option value="Sick">Sick Leave</Option>
              <Option value="Casual">Casual Leave</Option>
              <Option value="Annual">Annual Leave</Option>
              <Option value="Unpaid">Unpaid Leave</Option>
            </Select>
          </Form.Item>

          <Form.Item
            name="dateRange"
            label="Leave Period"
            rules={[{
              required: true,
              message:  'Please select dates',
            }]}
          >
            <RangePicker
              style={{ width: '100%' }}
              disabledDate={current =>
                current &&
                current < dayjs().startOf('day')}
            />
          </Form.Item>

          <Form.Item
            name="reason"
            label="Reason (optional)"
          >
            <TextArea
              rows={3}
              placeholder="Provide a reason..."
              maxLength={500}
              showCount
            />
          </Form.Item>

          <Form.Item style={{ marginBottom: 0 }}>
            <Space>
              <Button
                type="primary"
                htmlType="submit"
                loading={isPending}
              >
                Submit Request
              </Button>
              <Button onClick={() => navigate('/leave')}>
                Cancel
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </Card>
    </div>
  )
}
import {
  Card, Avatar, Tag, Button, Descriptions,
  Typography, Space, Tabs, Table, Timeline,
  Popconfirm, message, Spin, Empty,
} from 'antd'
import {
  UserOutlined, EditOutlined,
  DeleteOutlined, ArrowLeftOutlined,
} from '@ant-design/icons'
import {
  useQuery, useMutation, useQueryClient,
} from '@tanstack/react-query'
import { useParams, useNavigate } from 'react-router-dom'
import { employeeApi } from '../../api/employees'
import { leaveApi }    from '../../api/leaveRequests'
import client          from '../../api/client'
import type { AuditLog, LeaveRequest } from '../../types'
import dayjs from 'dayjs'

const { Title, Text } = Typography

export default function EmployeeDetailPage() {
  const { id }        = useParams<{ id: string }>()
  const navigate      = useNavigate()
  const queryClient   = useQueryClient()
  const employeeId    = parseInt(id!)

  // ── Queries ───────────────────────────────────────────────
  const { data: employee, isLoading } = useQuery({
    queryKey: ['employee', employeeId],
    queryFn:  () => employeeApi.getById(employeeId),
  })

  const { data: leaveRequests } = useQuery({
    queryKey: ['leave', 'employee', employeeId],
    queryFn:  () => leaveApi.getByEmployee(employeeId),
  })

  const { data: auditLogs } = useQuery({
    queryKey: ['audit', 'Employee', employeeId],
    queryFn:  () => client
      .get<AuditLog[]>(
        `/auditlogs/Employee/${employeeId}`)
      .then(r => r.data),
  })

  // ── Delete Mutation ───────────────────────────────────────
  const { mutate: deleteEmployee } = useMutation({
    mutationFn: () => employeeApi.delete(employeeId),
    onSuccess: () => {
      message.success('Employee deactivated')
      queryClient.invalidateQueries({ queryKey: ['employees'] })
      navigate('/employees')
    },
  })

  if (isLoading) return (
    <div style={{ textAlign: 'center', padding: 48 }}>
      <Spin size="large" />
    </div>
  )

  if (!employee) return (
    <Empty description="Employee not found" />
  )

  // ── Leave Table Columns ───────────────────────────────────
  const leaveColumns = [
    {
      title:     'Type',
      dataIndex: 'leaveType',
      key:       'leaveType',
    },
    {
      title:  'Dates',
      key:    'dates',
      render: (_: unknown, r: LeaveRequest) =>
        `${dayjs(r.startDate).format('MMM D')} - ${dayjs(r.endDate).format('MMM D, YYYY')}`,
    },
    {
      title:     'Days',
      dataIndex: 'totalDays',
      key:       'totalDays',
    },
    {
      title:  'Status',
      key:    'status',
      render: (_: unknown, r: LeaveRequest) => {
        const colors: Record<string, string> = {
          Pending:   'orange',
          Approved:  'green',
          Rejected:  'red',
          Cancelled: 'default',
        }
        return (
          <Tag color={colors[r.status] || 'default'}>
            {r.status}
          </Tag>
        )
      },
    },
  ]

  return (
    <div>
      {/* Header */}
      <div className="page-header">
        <Space>
          <Button
            icon={<ArrowLeftOutlined />}
            onClick={() => navigate('/employees')}
          >
            Back
          </Button>
          <Title level={3} style={{ margin: 0 }}>
            Employee Detail
          </Title>
        </Space>
        <Space>
          <Button
            icon={<EditOutlined />}
            onClick={() =>
              navigate(`/employees/${employeeId}/edit`)}
          >
            Edit
          </Button>
          <Popconfirm
            title="Deactivate this employee?"
            description="This will soft-delete the employee."
            onConfirm={() => deleteEmployee()}
            okText="Yes"
            cancelText="No"
          >
            <Button
              danger
              icon={<DeleteOutlined />}
            >
              Deactivate
            </Button>
          </Popconfirm>
        </Space>
      </div>

      {/* Profile Card */}
      <Card style={{ marginBottom: 16 }}>
        <Space size={24} align="start">
          <Avatar
            size={80}
            src={employee.avatarUrl}
            icon={<UserOutlined />}
            style={{ backgroundColor: '#1677ff' }}
          />
          <div>
            <Title level={4} style={{ margin: 0 }}>
              {employee.fullName}
            </Title>
            <Text type="secondary">
              {employee.designationName}
            </Text>
            <br />
            <Text type="secondary">
              {employee.departmentName}
            </Text>
            <br />
            <Tag
              color={employee.isActive
                ? 'success' : 'error'}
              style={{ marginTop: 8 }}
            >
              {employee.isActive ? 'Active' : 'Inactive'}
            </Tag>
          </div>
        </Space>
      </Card>

      {/* Tabs */}
      <Tabs
        defaultActiveKey="profile"
        items={[
          {
            key:   'profile',
            label: 'Profile',
            children: (
              <Card>
                <Descriptions
                  bordered
                  column={2}
                  size="small"
                >
                  <Descriptions.Item label="Email">
                    {employee.email}
                  </Descriptions.Item>
                  <Descriptions.Item label="Phone">
                    {employee.phone || '—'}
                  </Descriptions.Item>
                  <Descriptions.Item label="City">
                    {employee.city || '—'}
                  </Descriptions.Item>
                  <Descriptions.Item label="Country">
                    {employee.country || '—'}
                  </Descriptions.Item>
                  <Descriptions.Item
                    label="Address"
                    span={2}
                  >
                    {employee.address || '—'}
                  </Descriptions.Item>
                  <Descriptions.Item label="Salary">
                    ${employee.salary.toLocaleString()}
                  </Descriptions.Item>
                  <Descriptions.Item
                    label="Joining Date"
                  >
                    {dayjs(employee.joiningDate)
                      .format('MMMM D, YYYY')}
                  </Descriptions.Item>
                  <Descriptions.Item
                    label="Skills"
                    span={2}
                  >
                    <Space wrap>
                      {employee.skills.map(s => (
                        <Tag key={s} color="blue">
                          {s}
                        </Tag>
                      ))}
                    </Space>
                  </Descriptions.Item>
                </Descriptions>
              </Card>
            ),
          },
          {
            key:   'leave',
            label: `Leave History (${leaveRequests?.length ?? 0})`,
            children: (
              <Table
                rowKey="id"
                columns={leaveColumns}
                dataSource={leaveRequests}
                pagination={false}
                locale={{
                  emptyText: 'No leave requests found',
                }}
              />
            ),
          },
          {
            key:   'audit',
            label: `Audit Trail (${auditLogs?.length ?? 0})`,
            children: auditLogs?.length ? (
              <Timeline
                items={auditLogs.map(log => ({
                  color: 'blue',
                  children: (
                    <div>
                      <Text strong>
                        {log.eventType}
                      </Text>
                      <br />
                      <Text type="secondary"
                        style={{ fontSize: 12 }}>
                        {dayjs(log.occurredAt)
                          .format('MMM D, YYYY HH:mm')}
                        {log.actorName &&
                          ` — by ${log.actorName}`}
                      </Text>
                    </div>
                  ),
                }))}
              />
            ) : (
              <Empty
                description="No audit logs found"
              />
            ),
          },
        ]}
      />
    </div>
  )
}
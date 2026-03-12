import {
  Table, Tag, Button, Select, Space,
  Typography, Card, Descriptions, Modal,
  Form, Input, message, Empty, Timeline,
} from 'antd'
import {
  PlusOutlined, EyeOutlined,
} from '@ant-design/icons'
import {
  useQuery, useMutation, useQueryClient,
} from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { leaveApi } from '../../api/leaveRequests'
import type {
  LeaveRequest, UpdateLeaveStatusDto,
  LeaveQueryParams,
} from '../../types'
import dayjs from 'dayjs'
import { useState } from 'react'

const { Title, Text } = Typography
const { Option }      = Select
const { TextArea }    = Input

const STATUS_COLORS: Record<string, string> = {
  Pending:   'orange',
  Approved:  'green',
  Rejected:  'red',
  Cancelled: 'default',
}

export default function LeaveListPage() {
  const navigate    = useNavigate()
  const queryClient = useQueryClient()

  const [filters, setFilters] =
    useState<LeaveQueryParams>({})
  const [selected, setSelected] =
    useState<LeaveRequest | null>(null)
  const [actionModal, setActionModal] =
    useState<'approve' | 'reject' | null>(null)
  const [detailModal, setDetailModal] =
    useState(false)
  const [form] = Form.useForm()

  // ── Query ─────────────────────────────────────────────────
  const { data: requests, isLoading } = useQuery({
    queryKey: ['leaveRequests', filters],
    queryFn:  () => leaveApi.getAll(filters),
  })

  // ── Status Update Mutation ────────────────────────────────
  const { mutate: updateStatus, isPending } =
    useMutation({
      mutationFn: ({
        id, dto,
      }: {
        id:  string
        dto: UpdateLeaveStatusDto
      }) => leaveApi.updateStatus(id, dto),
      onSuccess: () => {
        message.success('Status updated successfully')
        queryClient.invalidateQueries({
          queryKey: ['leaveRequests'],
        })
        setActionModal(null)
        setSelected(null)
        form.resetFields()
      },
    })

  // ── Cancel Mutation ───────────────────────────────────────
  const { mutate: cancelRequest } = useMutation({
    mutationFn: (id: string) =>
      leaveApi.cancel(id, 'Admin'),
    onSuccess: () => {
      message.success('Request cancelled')
      queryClient.invalidateQueries({
        queryKey: ['leaveRequests'],
      })
    },
  })

  // ── Handle Approve/Reject ─────────────────────────────────
  const handleAction = (values: Record<string, string>) => {
    if (!selected || !actionModal) return

    updateStatus({
      id: selected.id,
      dto: {
        status:    actionModal === 'approve'
          ? 'Approved' : 'Rejected',
        comment:   values.comment,
        actorName: values.actorName || 'Admin',
      },
    })
  }

  // ── Columns ───────────────────────────────────────────────
  const columns = [
    {
      title:     'Employee',
      dataIndex: 'employeeName',
      key:       'employeeName',
      render:    (name: string) => (
        <Text strong>{name}</Text>
      ),
    },
    {
      title:     'Leave Type',
      dataIndex: 'leaveType',
      key:       'leaveType',
      render:    (type: string) => (
        <Tag color="blue">{type}</Tag>
      ),
    },
    {
      title:  'Dates',
      key:    'dates',
      render: (_: unknown, r: LeaveRequest) => (
        <Space direction="vertical" size={0}>
          <Text style={{ fontSize: 13 }}>
            {dayjs(r.startDate).format('MMM D')} —{' '}
            {dayjs(r.endDate).format('MMM D, YYYY')}
          </Text>
          <Text
            type="secondary"
            style={{ fontSize: 12 }}
          >
            {r.totalDays} day
            {r.totalDays > 1 ? 's' : ''}
          </Text>
        </Space>
      ),
    },
    {
      title:  'Status',
      key:    'status',
      render: (_: unknown, r: LeaveRequest) => (
        <Tag color={STATUS_COLORS[r.status] || 'default'}>
          {r.status}
        </Tag>
      ),
    },
    {
      title:  'Submitted',
      key:    'createdAt',
      render: (_: unknown, r: LeaveRequest) =>
        dayjs(r.createdAt).format('MMM D, YYYY'),
    },
    {
      title:  'Actions',
      key:    'actions',
      render: (_: unknown, r: LeaveRequest) => (
        <Space>
          <Button
            size="small"
            icon={<EyeOutlined />}
            onClick={() => {
              setSelected(r)
              setDetailModal(true)
            }}
          >
            View
          </Button>
          {r.status === 'Pending' && (
            <>
              <Button
                size="small"
                type="primary"
                onClick={() => {
                  setSelected(r)
                  setActionModal('approve')
                }}
              >
                Approve
              </Button>
              <Button
                size="small"
                danger
                onClick={() => {
                  setSelected(r)
                  setActionModal('reject')
                }}
              >
                Reject
              </Button>
            </>
          )}
          {(r.status === 'Pending' ||
            r.status === 'Approved') && (
            <Button
              size="small"
              onClick={() => cancelRequest(r.id)}
            >
              Cancel
            </Button>
          )}
        </Space>
      ),
    },
  ]

  return (
    <div>
      {/* Header */}
      <div className="page-header">
        <Title level={3} style={{ margin: 0 }}>
          Leave Requests
        </Title>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => navigate('/leave/new')}
        >
          New Request
        </Button>
      </div>

      {/* Filters */}
      <Card style={{ marginBottom: 16 }}>
        <Space wrap>
          <Select
            placeholder="Filter by Status"
            allowClear
            style={{ width: 160 }}
            onChange={val => setFilters(f => ({
              ...f, status: val,
            }))}
          >
            <Option value="Pending">Pending</Option>
            <Option value="Approved">Approved</Option>
            <Option value="Rejected">Rejected</Option>
            <Option value="Cancelled">Cancelled</Option>
          </Select>

          <Select
            placeholder="Filter by Type"
            allowClear
            style={{ width: 160 }}
            onChange={val => setFilters(f => ({
              ...f, leaveType: val,
            }))}
          >
            <Option value="Sick">Sick</Option>
            <Option value="Casual">Casual</Option>
            <Option value="Annual">Annual</Option>
            <Option value="Unpaid">Unpaid</Option>
          </Select>
        </Space>
      </Card>

      {/* Table */}
      <Table
        rowKey="id"
        columns={columns}
        dataSource={requests}
        loading={isLoading}
        locale={{ emptyText: <Empty
          description="No leave requests found" />
        }}
      />

      {/* Detail Modal */}
      <Modal
        title="Leave Request Detail"
        open={detailModal}
        onCancel={() => {
          setDetailModal(false)
          setSelected(null)
        }}
        footer={null}
        width={600}
      >
        {selected && (
          <div>
            <Descriptions
              bordered
              size="small"
              column={2}
              style={{ marginBottom: 24 }}
            >
              <Descriptions.Item label="Employee">
                {selected.employeeName}
              </Descriptions.Item>
              <Descriptions.Item label="Leave Type">
                <Tag color="blue">
                  {selected.leaveType}
                </Tag>
              </Descriptions.Item>
              <Descriptions.Item label="Start Date">
                {dayjs(selected.startDate)
                  .format('MMMM D, YYYY')}
              </Descriptions.Item>
              <Descriptions.Item label="End Date">
                {dayjs(selected.endDate)
                  .format('MMMM D, YYYY')}
              </Descriptions.Item>
              <Descriptions.Item label="Total Days">
                {selected.totalDays} day
                {selected.totalDays > 1 ? 's' : ''}
              </Descriptions.Item>
              <Descriptions.Item label="Status">
                <Tag color={
                  STATUS_COLORS[selected.status]
                  || 'default'}>
                  {selected.status}
                </Tag>
              </Descriptions.Item>
              {selected.reason && (
                <Descriptions.Item
                  label="Reason"
                  span={2}
                >
                  {selected.reason}
                </Descriptions.Item>
              )}
            </Descriptions>

            {/* Approval History */}
            <Title level={5}>Approval History</Title>
            <Timeline
              items={selected.approvalHistory.map(h => ({
                color: h.status === 'Approved'
                  ? 'green'
                  : h.status === 'Rejected'
                  ? 'red'
                  : h.status === 'Cancelled'
                  ? 'gray'
                  : 'blue',
                children: (
                  <div>
                    <Space>
                      <Tag color={
                        STATUS_COLORS[h.status]
                        || 'default'}>
                        {h.status}
                      </Tag>
                      <Text strong>
                        {h.actorName}
                      </Text>
                    </Space>
                    {h.comment && (
                      <div>
                        <Text type="secondary">
                          {h.comment}
                        </Text>
                      </div>
                    )}
                    <div>
                      <Text
                        type="secondary"
                        style={{ fontSize: 12 }}
                      >
                        {dayjs(h.changedAt)
                          .format('MMM D, YYYY HH:mm')}
                      </Text>
                    </div>
                  </div>
                ),
              }))}
            />
          </div>
        )}
      </Modal>

      {/* Approve / Reject Modal */}
      <Modal
        title={
          actionModal === 'approve'
            ? 'Approve Request'
            : 'Reject Request'
        }
        open={!!actionModal}
        onCancel={() => {
          setActionModal(null)
          form.resetFields()
        }}
        footer={null}
        width={440}
      >
        <Form
          form={form}
          layout="vertical"
          onFinish={handleAction}
          initialValues={{ actorName: 'Admin' }}
        >
          <Form.Item
            name="actorName"
            label="Your Name"
            rules={[{ required: true }]}
          >
            <Input placeholder="Enter your name" />
          </Form.Item>

          <Form.Item
            name="comment"
            label="Comment (optional)"
          >
            <TextArea
              rows={3}
              placeholder="Add a comment..."
            />
          </Form.Item>

          <Form.Item style={{ marginBottom: 0 }}>
            <Space style={{ float: 'right' }}>
              <Button onClick={() => {
                setActionModal(null)
                form.resetFields()
              }}>
                Cancel
              </Button>
              <Button
                type="primary"
                htmlType="submit"
                loading={isPending}
                danger={actionModal === 'reject'}
              >
                {actionModal === 'approve'
                  ? 'Approve'
                  : 'Reject'}
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  )
}
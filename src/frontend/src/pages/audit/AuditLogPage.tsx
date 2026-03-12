import {
  Table, Tag, Typography, Card, Select,
  Space, Input, Button, Modal, Descriptions,
  Empty,
} from 'antd'
import { EyeOutlined, SearchOutlined } from '@ant-design/icons'
import { useQuery } from '@tanstack/react-query'
import client from '../../api/client'
import type { AuditLog } from '../../types'
import dayjs from 'dayjs'
import { useState } from 'react'

const { Title, Text } = Typography
const { Option }      = Select

const AGGREGATE_TYPES = [
  'Employee', 'Project', 'TaskItem', 'LeaveRequest',
]

export default function AuditLogPage() {
  const [aggregateType, setAggregateType] =
    useState<string | undefined>()
  const [aggregateId, setAggregateId] =
    useState<string>('')
  const [limit, setLimit] = useState(50)
  const [selected, setSelected] =
    useState<AuditLog | null>(null)

  // ── Query ─────────────────────────────────────────────────
  const { data: logs, isLoading } = useQuery({
    queryKey: ['auditLogs', aggregateType,
      aggregateId, limit],
    queryFn: () => {
      const params: Record<string, unknown> = { limit }
      if (aggregateType) params.aggregateType = aggregateType
      if (aggregateId)   params.aggregateId   = aggregateId
      return client
        .get<AuditLog[]>('/auditlogs', { params })
        .then(r => r.data)
    },
  })

  // ── Columns ───────────────────────────────────────────────
  const columns = [
    {
      title:  'Event',
      key:    'event',
      render: (_: unknown, r: AuditLog) => (
        <Space direction="vertical" size={0}>
          <Tag color="blue">{r.eventType}</Tag>
        </Space>
      ),
    },
    {
      title:  'Entity',
      key:    'entity',
      render: (_: unknown, r: AuditLog) => (
        <Space direction="vertical" size={0}>
          <Text strong style={{ fontSize: 13 }}>
            {r.aggregateType}
          </Text>
          <Text
            type="secondary"
            style={{ fontSize: 12 }}
          >
            ID: {r.aggregateId}
          </Text>
        </Space>
      ),
    },
    {
      title:     'Actor',
      dataIndex: 'actorName',
      key:       'actorName',
      render:    (actor: string) => (
        <Text>{actor || 'system'}</Text>
      ),
    },
    {
      title:     'Description',
      dataIndex: 'description',
      key:       'description',
      render:    (desc: string) => (
        <Text
          type="secondary"
          style={{ fontSize: 13 }}
        >
          {desc || '—'}
        </Text>
      ),
    },
    {
      title:  'Time',
      key:    'time',
      width:  160,
      render: (_: unknown, r: AuditLog) => (
        <Text
          type="secondary"
          style={{ fontSize: 12 }}
        >
          {dayjs(r.occurredAt)
            .format('MMM D, YYYY HH:mm')}
        </Text>
      ),
    },
    {
      title:  'Action',
      key:    'action',
      width:  80,
      render: (_: unknown, r: AuditLog) => (
        <Button
          size="small"
          icon={<EyeOutlined />}
          onClick={() => setSelected(r)}
        >
          View
        </Button>
      ),
    },
  ]

  return (
    <div>
      {/* Header */}
      <div className="page-header">
        <Title level={3} style={{ margin: 0 }}>
          Audit Log
        </Title>
        <Text type="secondary">
          {logs?.length ?? 0} records
        </Text>
      </div>

      {/* Filters */}
      <Card style={{ marginBottom: 16 }}>
        <Space wrap>
          <Select
            placeholder="Filter by Entity Type"
            allowClear
            style={{ width: 180 }}
            onChange={val => setAggregateType(val)}
          >
            {AGGREGATE_TYPES.map(t => (
              <Option key={t} value={t}>{t}</Option>
            ))}
          </Select>

          <Input
            placeholder="Entity ID"
            prefix={<SearchOutlined />}
            style={{ width: 140 }}
            value={aggregateId}
            onChange={e =>
              setAggregateId(e.target.value)}
            allowClear
          />

          <Select
            value={limit}
            style={{ width: 120 }}
            onChange={val => setLimit(val)}
          >
            <Option value={25}>25 records</Option>
            <Option value={50}>50 records</Option>
            <Option value={100}>100 records</Option>
          </Select>
        </Space>
      </Card>

      {/* Table */}
      <Table
        rowKey="id"
        columns={columns}
        dataSource={logs}
        loading={isLoading}
        pagination={{ pageSize: 20 }}
        locale={{
          emptyText: (
            <Empty
              description="No audit logs yet. Audit logs are generated when the Audit Worker processes domain events."
            />
          ),
        }}
      />

      {/* Detail Modal */}
      <Modal
        title="Audit Log Detail"
        open={!!selected}
        onCancel={() => setSelected(null)}
        footer={null}
        width={600}
      >
        {selected && (
          <Descriptions
            bordered
            size="small"
            column={1}
          >
            <Descriptions.Item label="Event Type">
              <Tag color="blue">
                {selected.eventType}
              </Tag>
            </Descriptions.Item>
            <Descriptions.Item label="Entity">
              {selected.aggregateType} #
              {selected.aggregateId}
            </Descriptions.Item>
            <Descriptions.Item label="Actor">
              {selected.actorName || 'system'}
            </Descriptions.Item>
            <Descriptions.Item label="Description">
              {selected.description || '—'}
            </Descriptions.Item>
            <Descriptions.Item label="Occurred At">
              {dayjs(selected.occurredAt)
                .format('MMMM D, YYYY HH:mm:ss')}
            </Descriptions.Item>
            {selected.before && (
              <Descriptions.Item label="Before">
                <pre style={{
                  fontSize:   12,
                  background: '#f5f5f5',
                  padding:    8,
                  borderRadius: 4,
                  overflow:   'auto',
                  maxHeight:  200,
                }}>
                  {JSON.stringify(
                    JSON.parse(selected.before),
                    null, 2)}
                </pre>
              </Descriptions.Item>
            )}
            {selected.after && (
              <Descriptions.Item label="After">
                <pre style={{
                  fontSize:   12,
                  background: '#f5f5f5',
                  padding:    8,
                  borderRadius: 4,
                  overflow:   'auto',
                  maxHeight:  200,
                }}>
                  {JSON.stringify(
                    JSON.parse(selected.after),
                    null, 2)}
                </pre>
              </Descriptions.Item>
            )}
          </Descriptions>
        )}
      </Modal>
    </div>
  )
}
import {
  Card, Button, Tag, Typography, Row, Col,
  Progress, Space, Empty, Spin, Modal,
  Form, Input, Select, DatePicker, message,
} from 'antd'
import {
  PlusOutlined, TeamOutlined, CheckSquareOutlined,
} from '@ant-design/icons'
import {
  useQuery, useMutation, useQueryClient,
} from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { projectApi } from '../../api/projects'
import type { CreateProjectDto } from '../../types'
import dayjs from 'dayjs'
import { useState } from 'react'

const { Title, Text } = Typography
const { Option }      = Select
const { TextArea }    = Input

const STATUS_COLORS: Record<string, string> = {
  Active:    'blue',
  OnHold:    'orange',
  Completed: 'green',
  Cancelled: 'red',
}

export default function ProjectListPage() {
  const navigate    = useNavigate()
  const queryClient = useQueryClient()
  const [form]      = Form.useForm()
  const [open, setOpen] = useState(false)

  // ── Query ─────────────────────────────────────────────────
  const { data: projects, isLoading } = useQuery({
    queryKey: ['projects'],
    queryFn:  projectApi.getAll,
  })

  // ── Create Mutation ───────────────────────────────────────
  const { mutate: createProject, isPending } = useMutation({
    mutationFn: (dto: CreateProjectDto) =>
      projectApi.create(dto),
    onSuccess: (project) => {
      message.success('Project created successfully')
      queryClient.invalidateQueries({ queryKey: ['projects'] })
      setOpen(false)
      form.resetFields()
      navigate(`/projects/${project.id}`)
    },
  })

  const onFinish = (values: Record<string, unknown>) => {
    const dto: CreateProjectDto = {
      name:        values.name        as string,
      description: values.description as string,
      status:      values.status      as string,
      startDate:   dayjs(
        values.startDate as string)
        .format('YYYY-MM-DD'),
      endDate: values.endDate
        ? dayjs(values.endDate as string)
            .format('YYYY-MM-DD')
        : undefined,
      memberIds: [],
    }
    createProject(dto)
  }

  if (isLoading) return (
    <div style={{ textAlign: 'center', padding: 48 }}>
      <Spin size="large" />
    </div>
  )

  return (
    <div>
      {/* Header */}
      <div className="page-header">
        <Title level={3} style={{ margin: 0 }}>
          Projects
        </Title>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => setOpen(true)}
        >
          New Project
        </Button>
      </div>

      {/* Project Cards */}
      {projects?.length === 0 ? (
        <Empty description="No projects found" />
      ) : (
        <Row gutter={[16, 16]}>
          {projects?.map(project => {
            const progress = project.taskCount > 0
              ? Math.round(
                  (project.completedTaskCount /
                    project.taskCount) * 100)
              : 0

            return (
              <Col xs={24} sm={12} lg={8} key={project.id}>
                <Card
                  hoverable
                  onClick={() =>
                    navigate(`/projects/${project.id}`)}
                  style={{ height: '100%' }}
                  actions={[
                    <Space key="members">
                      <TeamOutlined />
                      <Text>{project.memberCount} members</Text>
                    </Space>,
                    <Space key="tasks">
                      <CheckSquareOutlined />
                      <Text>
                        {project.completedTaskCount}/
                        {project.taskCount} tasks
                      </Text>
                    </Space>,
                  ]}
                >
                  {/* Status Tag */}
                  <div style={{
                    display:        'flex',
                    justifyContent: 'space-between',
                    alignItems:     'center',
                    marginBottom:   8,
                  }}>
                    <Tag color={
                      STATUS_COLORS[project.status]
                        || 'default'}>
                      {project.status}
                    </Tag>
                    <Text
                      type="secondary"
                      style={{ fontSize: 12 }}
                    >
                      {dayjs(project.startDate)
                        .format('MMM D, YYYY')}
                    </Text>
                  </div>

                  {/* Name */}
                  <Title level={5} style={{ margin: '8px 0' }}>
                    {project.name}
                  </Title>

                  {/* Description */}
                  <Text
                    type="secondary"
                    style={{
                      fontSize:   13,
                      display:    '-webkit-box',
                      WebkitLineClamp: 2,
                      WebkitBoxOrient: 'vertical',
                      overflow:   'hidden',
                    }}
                  >
                    {project.description || 'No description'}
                  </Text>

                  {/* Progress */}
                  <div style={{ marginTop: 16 }}>
                    <div style={{
                      display:        'flex',
                      justifyContent: 'space-between',
                      marginBottom:   4,
                    }}>
                      <Text
                        type="secondary"
                        style={{ fontSize: 12 }}
                      >
                        Progress
                      </Text>
                      <Text
                        type="secondary"
                        style={{ fontSize: 12 }}
                      >
                        {progress}%
                      </Text>
                    </div>
                    <Progress
                      percent={progress}
                      showInfo={false}
                      strokeColor={
                        progress === 100
                          ? '#52c41a' : '#1677ff'}
                      size="small"
                    />
                  </div>
                </Card>
              </Col>
            )
          })}
        </Row>
      )}

      {/* Create Project Modal */}
      <Modal
        title="Create New Project"
        open={open}
        onCancel={() => {
          setOpen(false)
          form.resetFields()
        }}
        footer={null}
        width={600}
      >
        <Form
          form={form}
          layout="vertical"
          onFinish={onFinish}
          initialValues={{ status: 'Active' }}
        >
          <Form.Item
            name="name"
            label="Project Name"
            rules={[{ required: true }]}
          >
            <Input placeholder="Enter project name" />
          </Form.Item>

          <Form.Item
            name="description"
            label="Description"
          >
            <TextArea
              rows={3}
              placeholder="Project description..."
            />
          </Form.Item>

          <Form.Item
            name="status"
            label="Status"
            rules={[{ required: true }]}
          >
            <Select>
              <Option value="Active">Active</Option>
              <Option value="OnHold">On Hold</Option>
              <Option value="Completed">Completed</Option>
              <Option value="Cancelled">Cancelled</Option>
            </Select>
          </Form.Item>

          <Form.Item
            name="startDate"
            label="Start Date"
            rules={[{ required: true }]}
          >
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item name="endDate" label="End Date">
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item style={{ marginBottom: 0 }}>
            <Space style={{ float: 'right' }}>
              <Button onClick={() => {
                setOpen(false)
                form.resetFields()
              }}>
                Cancel
              </Button>
              <Button
                type="primary"
                htmlType="submit"
                loading={isPending}
              >
                Create Project
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  )
}
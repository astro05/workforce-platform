import { useState } from 'react'
import { Layout, Menu } from 'antd'
import { useNavigate, useLocation, Outlet } from 'react-router-dom'
import {
  DashboardOutlined,
  TeamOutlined,
  ProjectOutlined,
  CalendarOutlined,
  AuditOutlined,
} from '@ant-design/icons'

const { Sider, Content, Header } = Layout

export default function AppLayout() {
  const navigate  = useNavigate()
  const location  = useLocation()
  const [collapsed, setCollapsed] = useState(false)

  const menuItems = [
    {
      key:   '/',
      icon:  <DashboardOutlined />,
      label: 'Dashboard',
    },
    {
      key:   '/employees',
      icon:  <TeamOutlined />,
      label: 'Employees',
    },
    {
      key:   '/projects',
      icon:  <ProjectOutlined />,
      label: 'Projects',
    },
    {
      key:   '/leave',
      icon:  <CalendarOutlined />,
      label: 'Leave Requests',
    },
    {
      key:   '/audit',
      icon:  <AuditOutlined />,
      label: 'Audit Logs',
    },
  ]

  // Highlight correct menu item
  const selectedKey = '/' + location.pathname.split('/')[1]

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider
        collapsible
        collapsed={collapsed}
        onCollapse={setCollapsed}
        style={{ background: '#001529' }}
      >
        {/* Logo */}
        <div style={{
          height:     64,
          display:    'flex',
          alignItems: 'center',
          justifyContent: 'center',
          color:      '#fff',
          fontSize:   collapsed ? 14 : 18,
          fontWeight: 'bold',
          padding:    '0 16px',
          borderBottom: '1px solid #1f2f3f',
        }}>
          {collapsed ? 'WF' : 'Workforce'}
        </div>

        <Menu
          theme="dark"
          mode="inline"
          selectedKeys={[selectedKey]}
          items={menuItems}
          onClick={({ key }) => navigate(key)}
          style={{ marginTop: 8 }}
        />
      </Sider>

      <Layout>
        <Header style={{
          background:   '#fff',
          padding:      '0 24px',
          borderBottom: '1px solid #f0f0f0',
          display:      'flex',
          alignItems:   'center',
        }}>
          <span style={{
            fontSize:   20,
            fontWeight: 600,
            color:      '#1677ff',
          }}>
            Workforce Management Platform
          </span>
        </Header>

        <Content style={{
          margin:     '24px',
          padding:    '24px',
          background: '#fff',
          borderRadius: 8,
          minHeight:  280,
        }}>
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  )
}